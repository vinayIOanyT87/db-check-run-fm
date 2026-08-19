import { Component, OnInit, OnDestroy, ChangeDetectorRef, ApplicationRef } from '@angular/core';
import { RowProperties } from '../services/DTO/row-properties';
import { TransactionAliasField, TransactionAliasPlacementInformation } from '../../fm-core/DTO/transaction-alias-placement-information';
import { Subscription, combineLatest, of } from 'rxjs';
import { SiteService } from '../../fm-core/services/site.service';
import { CurrentTransactionDetailsService } from '../services/current-transaction-details.service';
import { LoadingScreenService } from '../../fm-core/services/loading-screen.service';
import { PlacementEngineService } from '../services/placement-engine.service';
import { TransactionService } from '../../fm-core/services/transaction.service';
import { map, filter, mergeMap } from 'rxjs/operators';
import { CurrentTransactionFieldBagService } from '../services/current-transaction-field-bag.service';
import { MeterMathService } from '../services/meter-math.service';
import { SaveMeterStartOnMeterService } from '../services/save-meter-start-on-meter.service';
import { VCFFieldCalculatorService } from '../services/vcffield-calculator.service';
import { TransactionInSimplifiedFormat } from '../../fm-core/DTO/transaction-in-simplified-format';

@Component({
  selector: 'app-insert-transaction',
  templateUrl: './insert-transaction.component.html',
  styleUrls: ['./insert-transaction.component.css']
})
export class InsertTransactionComponent implements OnInit, OnDestroy {
  public rowCount: number;
  public columnCount: number;
  public tableToRender: RowProperties[] = [];
  public allTransactionFields: TransactionAliasField[] = [];
  public transactionAliasName = '';
  public currentUser = '';
  public transactionAliasGuid = '';
  public loading = true;
  public existingTransactionGuid = '';
  public retrieveExistingTransaction = false;
  public previousUrl = '';
  public canEdit = true;
  public canReverse = false;
  public reversalType = '';
  public currentlyInReverseUpdate = false;

  private copyRenderEngineToLocalSubscription: Subscription;
  public focusField = '';

  private meterMathSubscription: Subscription;
  private meterStartService: Subscription;
  private vcfCalculations: Subscription;

  constructor(private _siteService: SiteService,
    private _currentTransactionDetailsService: CurrentTransactionDetailsService,
    private _loadingScreenService: LoadingScreenService,
    private _placementEngine: PlacementEngineService,
    private _transactionService: TransactionService,
    private _transactionBag: CurrentTransactionFieldBagService,
    private _meterMathService: MeterMathService,
    private _saveMeterStartOnMeterService: SaveMeterStartOnMeterService,
    private _vcfFieldCalculatorService: VCFFieldCalculatorService,
    private _changeRef: ChangeDetectorRef,
    private _applicationRef: ApplicationRef) {}

  setupPageServices(): void {
    this.meterMathSubscription = this._meterMathService.bootstrap().subscribe(() => {
      this._applicationRef.tick();
    });
    this.meterStartService =  this._saveMeterStartOnMeterService
      .bootstrap(this.currentUser, this._placementEngine.retrieveMeterStartOnMeterIDSet)
      .subscribe();
    this.vcfCalculations = this._vcfFieldCalculatorService.bootstrap().subscribe();
  }
  getInitialData(): void {
    // we need both the authorization and the current transaction alias to look for
    this._loadingScreenService.showLoadingScreen(1000);
    const currentAlias = this._currentTransactionDetailsService.getTransactionAliasName();
    this.retrieveExistingTransaction = this._currentTransactionDetailsService.getModifyTransaction();
    this.existingTransactionGuid = this._currentTransactionDetailsService.getExistingTransactionGuid();
    this.previousUrl = this._currentTransactionDetailsService.getPreviousUrl();
    this._siteService.authorization
      .pipe(map(siteAuth => ({ siteAuth: siteAuth, currentAliasName: currentAlias })))
      .pipe(filter(x => {
        return (x.siteAuth != null);
      }))
      .pipe(map(x => {
        const transactionAlias = x.siteAuth.Transactions.find(transaction => {
          return transaction._ID === x.currentAliasName;
        });
        if (!transactionAlias) {
          throw new Error('Could not find the transaction: ' + x.currentAliasName);
        }
        this.currentUser = x.siteAuth.SecurityProperties.UserID;
        this.transactionAliasName = transactionAlias._ID;
        return transactionAlias._IdentityGuid;
      }))
      .pipe(mergeMap(x => {
        this.transactionAliasGuid = x;
        return combineLatest(
          [this._placementEngine.getInitialData(x, true),
          this.retrieveExistingTransaction ?
          this._transactionService.getExistingTransaction(this.existingTransactionGuid) :
          of<TransactionInSimplifiedFormat>(null)])
          .pipe( map(results =>
                    ({ placementEngineOutput: results[0], existingTransaction: results[1] }) ) );
      }))
      .subscribe((x) => {
        this._transactionBag.addOrSetField('LookupTransactionStatusIndex', 'Completed', 'InsertTransactionComponent');
        this.focusField = 'Product';
        this._changeRef.detectChanges();
        if (this.retrieveExistingTransaction) {
          this._transactionBag.currentSettings.existingTransaction = true;
          const existingTransaction = x.existingTransaction.TransactionPropertyValuePairs;
          this.canEdit = x.existingTransaction.CanBeEdited;
          this.reversalType = x.existingTransaction.ReversalType;
          this.canReverse = x.existingTransaction.CanBeReversed;
          Object.keys(existingTransaction).forEach((key) => {
            this._transactionBag
              .addOrSetField(key, existingTransaction[key],
                'InsertTransactionComponent', true);
          });
          this.setupPageServices();
          this.loading = false;
          this._applicationRef.tick();
        } else {
          // ugly hack to prep up the automatic auto document number
          this._transactionBag.addOrSetField('DocumentNumber', '', 'InsertTransactionComponent');
          this.setupPageServices();

          if (this._currentTransactionDetailsService.isExtendedAddScenario()) {
            const toApply = this._currentTransactionDetailsService.presetFollowingFields();
            Object.keys(toApply).forEach(objectKey => {
              this._transactionBag
              .addOrSetField(objectKey, toApply[objectKey],
                'InsertTransactionComponent', true);
            });
          }
          this._loadingScreenService.hideLoadingScreen();
          this.loading = false;
          this._applicationRef.tick();
        }
      });
  }

  getTransactionFieldDetails(toLookup: TransactionAliasPlacementInformation): TransactionAliasField {
    if (this.allTransactionFields === null || this.allTransactionFields === undefined) {
      return null;
    }
    if (toLookup === null || toLookup === undefined) {
      return null;
    }
    return this.allTransactionFields
      .find(x => x.IdentityGuid === toLookup.identityGuid);
  }

  async ngOnInit(): Promise<void> {
    this._transactionBag.clear();
    this.getInitialData();
    this.copyRenderEngineToLocalSubscription = this._placementEngine.fieldsHaveChanged.subscribe(() => {
      console.log('this._placementEngine.fieldsHaveChanged.subscribe');
      this.tableToRender = this._placementEngine.tableToRender;
      this.allTransactionFields = this._placementEngine.allTransactionFields;
      this._applicationRef.tick();
    });
  }

  ngOnDestroy(): void {
    this._transactionBag.clear();
    this.meterMathSubscription.unsubscribe();
    this.meterStartService.unsubscribe();
    this.vcfCalculations.unsubscribe();
    this.copyRenderEngineToLocalSubscription.unsubscribe();
  }

  log(toLog: string): void {
    console.log(toLog);
  }

  async save(): Promise<void> {
    const enchancedAddScenarioWithReturnUrl = this._currentTransactionDetailsService.isExtendedAddScenario();
    this._vcfFieldCalculatorService.disableVcfUpdatesFor1Second();
    this._saveMeterStartOnMeterService.save();
    const fieldsToSave = this._transactionBag.getAllProperties();
    if (!(this.retrieveExistingTransaction || enchancedAddScenarioWithReturnUrl)) {
      this._meterMathService.reset();
      this._transactionBag.clear();
    }
    const fieldNamesToSave = this._placementEngine.fieldsThatNeedToCopyToNextTransaction();
    fieldNamesToSave.forEach((x) => {
      const fieldToEmit = fieldsToSave[x];
      this._transactionBag.addOrSetField(x, fieldsToSave[x], 'InsertTransactionComponent');
    });
    if (this._transactionBag.getField('LookupTransactionStatusIndex') === null) {
      this._transactionBag.addOrSetField('LookupTransactionStatusIndex', 'Completed', 'InsertTransactionComponent');
    }
    this._vcfFieldCalculatorService.checkForDisabledFields();
    this.focusField = '';
    setTimeout(() => {
      this.focusField = 'DocumentNumber';
      this._changeRef.detectChanges();
    }, 100);

    if (!this.retrieveExistingTransaction && !enchancedAddScenarioWithReturnUrl) {
        this._transactionService.saveNewTransactionToBackground(fieldsToSave, this.transactionAliasGuid);
    } else if (enchancedAddScenarioWithReturnUrl) {
      this._loadingScreenService.showLoadingScreen(2000);
        await this._transactionService.saveNewTransactionToServer({
            sentEntry: fieldsToSave,
            sentTransactionAliasGuid: this.transactionAliasGuid,
            returnedEntry: null,
            isSent: true,
            timeStarted: null,
            timeComplete: null,
            error: null
          });
        this._loadingScreenService.hideLoadingScreen();
        window.location.href = this.previousUrl;
        this._changeRef.detectChanges();
    } else {
        this._loadingScreenService.showLoadingScreen(2000);
        await this._transactionService.updateExistingTransaction(fieldsToSave,
          this.transactionAliasGuid, this.existingTransactionGuid);
        this._loadingScreenService.hideLoadingScreen();
        window.location.href = this.previousUrl;
        this._changeRef.detectChanges();
      }
  }

  async delete(): Promise<void> {
    this._loadingScreenService.showLoadingScreen(2000);
    await this._transactionService.deleteExistingTransaction(this.existingTransactionGuid);
    this._loadingScreenService.hideLoadingScreen();
    window.location.href = this.previousUrl;
  }

  async reverse(): Promise<void> {
    this._loadingScreenService.showLoadingScreen(2000);
    await this._transactionService.reverseExistingTransaction(this.existingTransactionGuid);
    window.location.href = this.previousUrl;
  }

  async reverseUpdate(): Promise<void> {
    this.canEdit = true;
    this.currentlyInReverseUpdate = true;
    const fields = this._transactionBag.getAllProperties();
    let found = false;
    Object.keys(fields).forEach(x => {
      console.log(x);
      if (x === 'InventoryDate') { found = true; } });
    if (found) {
      console.log('asdfasdfasdfasdfadf');
      this._transactionBag.addOrSetField('InventoryDate', new Date().toISOString(), 'InsertTransactionComponent');
    }
  }

  async saveReverseUpdate(): Promise<void> {
    this._loadingScreenService.showLoadingScreen(3000);
    const fieldsToSave = this._transactionBag.getAllProperties();
    await this._transactionService.reverseUpdateTransaction(fieldsToSave,
      this.transactionAliasGuid, this.existingTransactionGuid);
    window.location.href = this.previousUrl;
  }

  sleep(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  close(): void {
    window.location.href = this.previousUrl;
  }
}
