import { Component, OnInit, OnDestroy, Input, ChangeDetectorRef } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField, ProductDTO } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { Subscription, onErrorResumeNext } from 'rxjs';
import { VCFFieldCalculatorService } from '../../../services/vcffield-calculator.service';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';
import { DecimalPipe } from '@angular/common';
import { CurrentTransactionSettings } from '../../../services/DTO/current-transaction-settings';

@Component({
  selector: 'app-transaction-vcfgrouping-input',
  templateUrl: './transaction-vcfgrouping-input.component.html',
  styleUrls: ['./transaction-vcfgrouping-input.component.css']
})
export class TransactionVCFGroupingInputComponent implements ITransactionAliasField, OnInit, OnDestroy {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public currentFocusLabel: string;
  @Input() public label = '';
  @Input() public fieldName = '';
  @Input() public canEdit: boolean;
  public required = false;

  private decimalPrecision: number = null;
  private _fieldContents: string = null;
  public set fieldContents(value: string) {
    if (value != null && value['replace']) { value = value.replace(/,/g, ''); }
    if (!isNaN(+value)) {
        this._fieldContents = value;
        this._fieldBagService.addOrSetField(this.fieldName, this._fieldContents);
    }
  }
  public get fieldContents(): string {
    const parsedFieldContents = +this._fieldContents;
    if (isNaN(parsedFieldContents) || this._fieldContents === '' || this._fieldContents == null) {
      return '';
    }
    return this._decimalPipe.transform(this._fieldContents, `1.${this.decimalPrecision}-${this.decimalPrecision}` ).replace(/,/g, '');
  }
  public isDisabled = false;
  public showLoading = false;

  private fieldUpdateSubscription: Subscription;
  private specificFieldUpdateSubscription: Subscription;
  private decimalUpdateSubscription = new Subscription;
  private loadingSubscription: Subscription;
  constructor(private _vcfService: VCFFieldCalculatorService,
      private _fieldBagService: CurrentTransactionFieldBagService,
      private _changeRef: ChangeDetectorRef,
      private _decimalPipe: DecimalPipe) { }

  isValid(): boolean {
    return true;
  }

  onBlur() {
    this.setVcfService();
  }

  ngOnInit(): void {
      if ((this.TransactionAliasField === null || this.TransactionAliasField === undefined)) {
          return;
      }
      this.label = this.TransactionAliasField.DisplayName;
      this.fieldName = this.TransactionAliasField.ID;
      this.required = this.TransactionAliasField.FieldRequired;

      if (this.fieldName === 'Vcf') {
          this.specificFieldUpdateSubscription = this._vcfService.vcf
          .subscribe(x => {
            if ((x.error === null || x.error === undefined)) {
              this.fieldContents = x.value;
              this.isDisabled = x.isDisabled;
              this._changeRef.detectChanges();
            } else {
              alert ('Please contact your local help desk regarding product setup.');
            }
          });

          this._vcfService.loadingVcfFromServer.subscribe(x => {
              this.showLoading = x;
              this._changeRef.detectChanges();
          });
      }

      if (this.fieldName === 'Temperature') {
          this.specificFieldUpdateSubscription = this._vcfService.temp.subscribe(x => {
              this.fieldContents = x.value;
              this.isDisabled = x.isDisabled;
              this._changeRef.detectChanges();
          });
      }

      if (this.fieldName === 'Density') {
          this.specificFieldUpdateSubscription = this._vcfService.density.subscribe(x => {
              this.fieldContents = x.value;
              this.isDisabled = x.isDisabled;
              this._changeRef.detectChanges();
          });
      }

      this.fieldUpdateSubscription = this._fieldBagService.hasChanged
      .pipe(filter((x) => x.field === this.fieldName))
      .pipe(filter((x) => x.source !== this.fieldName))
      .subscribe((x) => {
        if (x.value == null) {
          x.value = '';
        }
        this.fieldContents = x.value;
        this.setVcfService();
        this._changeRef.detectChanges();
      });

      this.decimalPrecision = this._fieldBagService.currentSettings.volumeDecimalPrecision;
      if (this.fieldName === 'Vcf') { this.decimalPrecision = 4; }
      this.decimalUpdateSubscription = this._fieldBagService.hasChanged
      .pipe(filter((x) => x.field === 'Product'))
      .subscribe((x) => {
        if (x.value === null || x.value === '') {
          this.setDecimalPrecisionForTransactionAlias(this._fieldBagService.currentSettings);
        } else {
          const product = this._fieldBagService.currentSettings.products.find(element => element.ID === x.value);
          if (!product) {
            throw new Error();
          }
          this.setDecimalPrecisionForProduct(product);
          this.fieldContents = this._fieldContents;
        }
        this._changeRef.detectChanges();
      });
  }

  setDecimalPrecisionForTransactionAlias(currentSettings: CurrentTransactionSettings): void {
    if (this.fieldName === 'Density') {
      this.decimalPrecision = currentSettings.densityDecimalPrecision;
    } else if (this.fieldName === 'Temperature') {
      this.decimalPrecision = currentSettings.temperatureDecimalPrecision;
    }
  }
  setDecimalPrecisionForProduct(currentProduct: ProductDTO): void {
    if (this.fieldName === 'Density') {
      this.decimalPrecision = currentProduct.DensityDecimalPlaces;
    } else if (this.fieldName === 'Temperature') {
      this.decimalPrecision = currentProduct.TemperatureDecimalPlaces;
    }
  }

  setVcfService(): void {
    if (this.fieldName === 'Vcf') {
      this._vcfService.setVcf(this.fieldContents);
    }
    if (this.fieldName === 'Temperature') {
        this._vcfService.setTemp(this.fieldContents);
    }
    if (this.fieldName === 'Density') {
        this._vcfService.setDensity(this.fieldContents);
    }
  }

  submit(): void {

  }

  ngOnDestroy(): void {
      this.fieldUpdateSubscription.unsubscribe();
      this.specificFieldUpdateSubscription.unsubscribe();
      this.decimalUpdateSubscription.unsubscribe();
      if (this.loadingSubscription) { this.loadingSubscription.unsubscribe(); }
  }
}
