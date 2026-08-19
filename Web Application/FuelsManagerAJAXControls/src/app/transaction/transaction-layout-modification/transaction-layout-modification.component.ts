import { Component, OnInit, OnDestroy, AfterViewInit, ApplicationRef } from '@angular/core';
import { Subscription, forkJoin, of } from 'rxjs';
import { RowProperties } from '../services/DTO/row-properties';
import { CurrentTransactionDetailsService } from '../services/current-transaction-details.service';
import { filter, map, mergeMap, delay, tap } from 'rxjs/operators';
import { PlacementEngineService } from '../services/placement-engine.service';
import { SiteService } from '../../fm-core/services/site.service';
import { TransactionService } from '../../fm-core/services/transaction.service';
import { TransactionAliasPlacementInformation } from '../../fm-core/DTO/transaction-alias-placement-information';

@Component({
  selector: 'app-transaction-layout-modification',
  templateUrl: './transaction-layout-modification.component.html',
  styleUrls: ['./transaction-layout-modification.component.css']
})
export class TransactionLayoutModificationComponent implements OnInit, OnDestroy, AfterViewInit {
  public saving = false;
  public saved = false;
  public tableToRender: RowProperties[] = [];
  public formFields: TransactionAliasPlacementInformation[] = [];
  public unplacedTransactionFields: TransactionAliasPlacementInformation[] = [];

  private transactionAliasName = '';
  private copyRenderEngineToLocalSubscription: Subscription;

  constructor(
      private _siteService: SiteService,
      private _transactionService: TransactionService,
      private _currentTransactionDetailsService: CurrentTransactionDetailsService,
      private _appRef: ApplicationRef,
      public _placementEngine: PlacementEngineService) {
      this.getInitialData();
      this.copyRenderEngineToLocalSubscription = this._placementEngine.fieldsHaveChanged.subscribe(() => {
          this.formFields = this._placementEngine.formFields;
          this.unplacedTransactionFields = this._placementEngine.unplacedTransactionFields;
          this.tableToRender = this._placementEngine.tableToRender;
      });
  }

  getInitialData(): void {
      // we need both the authorization and the current transaction alias to look for
      // this._loadingScreenService.showLoadingScreen(1000);
      const currentAlias = this._currentTransactionDetailsService.getTransactionAliasName();
      this._siteService.authorization
          .pipe(filter(x => {
              return (x != null);
          }))
          .pipe(map(x => {
              const transactionAlias = x.Transactions.find(transaction => {
                  return transaction._ID === currentAlias;
              });
              if (!transactionAlias) {
                  throw new Error('Could not find the transaction: ' + currentAlias);
              }
              this.transactionAliasName = transactionAlias._ID;
              return transactionAlias._IdentityGuid;
          }))
          .pipe(mergeMap(x => {
              return this._placementEngine.getInitialData(x);
          }))
          .subscribe(() => {
              console.log('done');
              this._appRef.tick();
              // this._loadingScreenService.hideLoadingScreen();
          });
  }


  // starting to move a TransactionAliasField
  onDragStart(event: any, data: TransactionAliasPlacementInformation) {
      if ((data === null || data === undefined)) {
          return;
      }

      if (data.isLine || data.isLabel) {
          data.identityGuid = Math.random().toString(36).substring(7);
      }
      event.dataTransfer.setData('text', JSON.stringify(data));
  }

  dropOntoPlacement(event: any, x: number, y: number): void {
      const toParse: string = event.dataTransfer.getData('text');
      if ((toParse === null || toParse === undefined) ||
          toParse === '') {
          return;
      }
      const dataTransfered: TransactionAliasPlacementInformation = JSON.parse(toParse);
      this._placementEngine.moveFieldPositionsToMap(x, y, dataTransfered);
      this._appRef.tick();

  }

  allowDrop(event: any) {
      event.stopPropagation();
      event.preventDefault();
      return false;
  }


  savePlacement(): void {
      this.saving = true;
      forkJoin(
          this._placementEngine.savePlacement(),
          of(null).pipe(delay(1000))
      )
          .pipe(tap(() => { this.saving = false; this.saved = true; }))
          .pipe(delay(3000))
          .subscribe(() => { this.saved = false; });
  }

  ngOnDestroy(): void {
      this.copyRenderEngineToLocalSubscription.unsubscribe();
  }

  ngOnInit(): void { }
  ngAfterViewInit(): void { }

}
