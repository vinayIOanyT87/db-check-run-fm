import { Injectable, ApplicationRef } from '@angular/core';
import { RowProperties, ColumnProperties } from './DTO/row-properties';
import { Subject, Observable, combineLatest } from 'rxjs';
import { take, tap, map } from 'rxjs/operators';
import { TransactionAliasPlacementInformation, TransactionAliasField, FieldWithAssociatedList } from '../../fm-core/DTO/transaction-alias-placement-information';
import { SiteService } from '../../fm-core/services/site.service';
import { TransactionService } from '../../fm-core/services/transaction.service';
import { TransactionAliasStoredFieldsEntry } from '../../fm-core/DTO/transaction-alias-stored-fields-entry';
import { CurrentTransactionFieldBagService } from './current-transaction-field-bag.service';
import { CurrentTransactionSettings } from './DTO/current-transaction-settings';

@Injectable({
  providedIn: 'root'
})
/** TransactionService
 * Calls the web api for common Transaction actions
 */
export class PlacementEngineService {
  private _rowCount: number;
  private _columnCount: number;

  public get rowCount(): number { return this._rowCount; }
  public set rowCount(count: number) {
      if (count > 0) {
          this._rowCount = count;
      }
  }
  public get columnCount(): number { return this._columnCount; }
  public set columnCount(count: number) {
      if (count > 0) {
          this._columnCount = count;
      }
  }

  public tableToRender: RowProperties[] = [];

  public formFields: TransactionAliasPlacementInformation[] = [];
  public unplacedTransactionFields: TransactionAliasPlacementInformation[] = [];
  public placedTransactionFields: TransactionAliasPlacementInformation[] = [];
  public allTransactionFields: TransactionAliasField[] = [];
  public currentTransactionSettings: CurrentTransactionSettings = new CurrentTransactionSettings();
  public transactionAliasGuid: string;
  public retrieveMeterStartOnMeterIDSet = false;

  private _fieldsHaveChanged: Subject<void> = new Subject();
  public fieldsHaveChanged: Observable<void> = this._fieldsHaveChanged.asObservable();

  constructor(private _siteService: SiteService,
      private transactionService: TransactionService,
      private _transactionBag: CurrentTransactionFieldBagService) {
  }


  private constructColumnsAndRows(): void {
      this.tableToRender.forEach(x => x.columns.length = 0);
      this.tableToRender.length = 0;
      for (let rows = 0; rows < this.rowCount; rows++) {
          const currentRow: RowProperties = new RowProperties({ rowIndex: rows });
          for (let columns = 0; columns < this.columnCount; columns++) {
              currentRow.columns.push(new ColumnProperties({
                  rowIndex: rows,
                  columnIndex: columns,
                  columnWidth: 1
              }));
          }
          currentRow.updateInternalCounters();
          this.tableToRender.push(currentRow);
      }

  }

  public async getInitialData(trans: string, defaultViewIfNoneIsSet: boolean = false): Promise<void> {
    this.SetInitialState(trans);
    // we need both the authorization and the current transaction alias to look for
    const transactionAliasPositionPromise = this.transactionService.getTransactionAliasPositions(this.transactionAliasGuid);
    const transactionAliasDetails = await this.transactionService.getTransactionAliasDetails(this.transactionAliasGuid);

    transactionAliasDetails.TransactionFields.forEach(y => {
      this.unplacedTransactionFields.push({
        id: y.ID,
        identityGuid: y.IdentityGuid,
        displayName: y.DisplayName,
        originalDisplayOrder: y.DisplayOrder,
        xPosition: null,
        yPosition: null,
        rowSpan: 0,
        columnSpan: 0,
        isPlaced: false,
        isLabel: false,
        isLine: false,
        labelContents: '',
        persistDataAfterSave: false
      });
    });

    this.allTransactionFields = transactionAliasDetails.TransactionFields;
    this._transactionBag.currentSettings.automaticDocumentNumber = transactionAliasDetails.AutoDocumentNumber;
    transactionAliasDetails.FieldsWithLists.forEach((z) => {
      this._transactionBag.addOrSetList(z.FieldName, z.Options, 'PlacementEngineService');
    });
    this._transactionBag.currentSettings.products = transactionAliasDetails.AllProducts;
    this._transactionBag.currentSettings.volumeDecimalPrecision = transactionAliasDetails.VolumeDecimalPrecision;
    this._transactionBag.currentSettings.temperatureDecimalPrecision = transactionAliasDetails.TemperatureDecimalPlaces;
    this._transactionBag.currentSettings.densityDecimalPrecision = transactionAliasDetails.DensityDecimalPlaces;
    this._transactionBag.currentSettings.transactionAliasGuid = this.transactionAliasGuid;
    this._transactionBag.currentSettings.transactionAliasType = transactionAliasDetails.TransactionAliasType;

    let transactionAliasPosition = await transactionAliasPositionPromise;
    if ((transactionAliasPosition === null || transactionAliasPosition === undefined)) {
      if (defaultViewIfNoneIsSet) {
        transactionAliasPosition = this.setupDefaultView(transactionAliasDetails.TransactionFields);
      } else {
        this.sortUnplacedTransactionFields();
        this._fieldsHaveChanged.next();
        return;
      }
    }

    this.columnCount = transactionAliasPosition.numberOfColumns;
    this.rowCount = transactionAliasPosition.numberOfRows;
    this.retrieveMeterStartOnMeterIDSet = transactionAliasPosition.retrieveMeterStartOnMeterIDSet;
    this.constructColumnsAndRows();
    if ((transactionAliasPosition.fieldMap === null || transactionAliasPosition.fieldMap === undefined)) {
      return;
    }
    transactionAliasPosition.fieldMap.forEach(value => {
      const foundFieldIndex = this.unplacedTransactionFields.findIndex(y => y.identityGuid === value.identityGuid);
      if (foundFieldIndex > -1) {
        this.unplacedTransactionFields.splice(foundFieldIndex, 1);
        this.placedTransactionFields.push(value);
        const cellToUpdate = this.getColumnDetails(value.xPosition, value.yPosition);
        if (!(cellToUpdate === null || cellToUpdate === undefined)) {
          cellToUpdate.columnWidth = value.columnSpan;
          cellToUpdate.rowHeightInEM = value.rowSpan;
          cellToUpdate.fieldToRender = value;
        }
      } else if (value.isLabel || value.isLine) {
        this.placedTransactionFields.push(value);
        const cellToUpdate = this.getColumnDetails(value.xPosition, value.yPosition);
        if (!(cellToUpdate === null || cellToUpdate === undefined)) {
          cellToUpdate.columnWidth = value.columnSpan;
          cellToUpdate.rowHeightInEM = value.rowSpan;
          cellToUpdate.fieldToRender = value;
        }
      } else { /* found a field that does not exist in the current definition, continue on */ }
    });
    this.tableToRender.forEach(z => z.updateInternalCounters());
    this.sortUnplacedTransactionFields();
    this._fieldsHaveChanged.next();
  }

  private SetInitialState(trans: string) {
    this.rowCount = 12;
    this.columnCount = 3;
    this.constructColumnsAndRows();
    this.formFields.length = 0;
    this.formFields.push({
      id: 'Label',
      identityGuid: 'Label',
      displayName: 'Label',
      originalDisplayOrder: -1,
      xPosition: null,
      yPosition: null,
      isPlaced: false,
      rowSpan: 0,
      columnSpan: 0,
      isLabel: true,
      isLine: false,
      labelContents: '',
      persistDataAfterSave: false
    });
    this.formFields.push({
      id: 'Horizontal Line',
      identityGuid: 'Horizontal Line',
      displayName: 'Horizontal Line',
      originalDisplayOrder: -1,
      xPosition: null,
      yPosition: null,
      isPlaced: false,
      rowSpan: 0,
      columnSpan: 0,
      isLabel: false,
      isLine: true,
      labelContents: '',
      persistDataAfterSave: false
    });
    this.transactionAliasGuid = trans;
    this.unplacedTransactionFields.length = 0;
    this.placedTransactionFields.length = 0;
    this.allTransactionFields.length = 0;
  }

  private setupDefaultView(fields: TransactionAliasField[]): TransactionAliasStoredFieldsEntry {
    const fieldsForTransaction = fields.slice(0);
    if (fieldsForTransaction.length > 0) {
      const rows = Math.ceil((fieldsForTransaction.length / 2));
      const newFieldMap: TransactionAliasPlacementInformation[] = [];
      for (let row = 0; row <= rows; row++) {
        for (let column = 0; column < 2; column++) {
          if (fieldsForTransaction.length <= 0) {
            continue;
          }
          const currentField = fieldsForTransaction.splice(0, 1)[0];
          const placmentInfo = {
            id: currentField.ID,
            identityGuid: currentField.IdentityGuid,
            displayName: currentField.DisplayName,
            originalDisplayOrder: currentField.DisplayOrder,
            xPosition: row,
            yPosition: column,
            isPlaced: true,
            columnSpan: 0,
            rowSpan: 0,
            isLabel: false,
            labelContents: '',
            isLine: false,
            persistDataAfterSave: false
          };
          newFieldMap.push(placmentInfo);
        }
      }
      return {
        aliasGuid: this.transactionAliasGuid,
        numberOfColumns: 2,
        numberOfRows: rows,
        fieldMap: newFieldMap,
        retrieveMeterStartOnMeterIDSet: false
      };
    }
    return null;
  }

  private sortUnplacedTransactionFields(): void {
      this.unplacedTransactionFields =
        this.unplacedTransactionFields.sort((x, y) => x.originalDisplayOrder > y.originalDisplayOrder ? 1 : -1);
  }

  public columnSpanUpdated(row: RowProperties): void {
      const itemsToReinsertIntoUnplacedItems = row.updateInternalCounters();
      itemsToReinsertIntoUnplacedItems.forEach(x => {
          this.deleteExistingEntryCurrentlyAtLocation(x.xPosition, x.yPosition);
      });
      this.sortUnplacedTransactionFields();
      this._fieldsHaveChanged.next();
  }

  private getColumnDetails(x: number, y: number): ColumnProperties {
      const row = this.tableToRender.find(z => z.rowIndex === x);
      if ((row === null || row === undefined)) {
          return null;
      }
      const column = row.columns.find(z => z.columnIndex === y);
      if ((column === null || column === undefined)) {
          return null;
      }
      return column;
  }

  public moveFieldPositionsToMap(xPosition: number, yPosition: number, fieldTransfered: TransactionAliasPlacementInformation): void {
      if (fieldTransfered.xPosition === xPosition && fieldTransfered.yPosition === yPosition) {
          // dropped right back to where it was, do nothing
          return;
      }
      this.deleteExistingEntryCurrentlyAtLocation(xPosition, yPosition);
      this.deleteExistingEntryCurrentlyAtLocation(fieldTransfered.xPosition, fieldTransfered.yPosition);

      // remove the field that is being moved from the bottom list
      const toRemove = this.unplacedTransactionFields.findIndex((field, index) => {
          return field.identityGuid === fieldTransfered.identityGuid;
      });
      if (toRemove > -1) {
          this.unplacedTransactionFields.splice(toRemove, 1);
      }

      // set the map
      fieldTransfered.xPosition = xPosition;
      fieldTransfered.yPosition = yPosition;
      fieldTransfered.isPlaced = true;
      fieldTransfered.rowSpan = 0;
      fieldTransfered.columnSpan = 0;
      this.placedTransactionFields.push(fieldTransfered);
      this.getColumnDetails(xPosition, yPosition).fieldToRender = fieldTransfered;

      this.sortUnplacedTransactionFields();
  }

  private deleteExistingEntryCurrentlyAtLocation(x: number, y: number): void {
      // if the map already has a key for this entry, delete it
      const currentColumnDetails = this.getColumnDetails(x, y);
      const fieldBeingReplaced = (currentColumnDetails === null || currentColumnDetails === undefined) ? null : currentColumnDetails.fieldToRender;
      if (fieldBeingReplaced) {
          currentColumnDetails.fieldToRender = null;
          fieldBeingReplaced.xPosition = null;
          fieldBeingReplaced.yPosition = null;
          fieldBeingReplaced.isPlaced = false;
          const position = this.placedTransactionFields.findIndex(z => z.identityGuid === fieldBeingReplaced.identityGuid);
          this.placedTransactionFields.splice(position, 1);
          if (!fieldBeingReplaced.isLabel && !fieldBeingReplaced.isLine) {
              this.unplacedTransactionFields.push(fieldBeingReplaced);
          }
          this.tableToRender.forEach(row => row.updateInternalCounters());
      }
  }


  private getTransactionAliasFieldConfigured(x: number, y: number): TransactionAliasPlacementInformation {
      return this.placedTransactionFields.find(z => z.xPosition === x && z.yPosition === y);
  }

  public async savePlacement(): Promise<void> {
      return await this.transactionService.saveTransactionAliasPositions({
          aliasGuid: this.transactionAliasGuid,
          fieldMap: this.placedTransactionFields,
          numberOfColumns: this.columnCount,
          numberOfRows: this.rowCount,
          retrieveMeterStartOnMeterIDSet: this.retrieveMeterStartOnMeterIDSet
      });
  }

  public autoPlaceFields(): void {
      this.resetPlacement();
      // each row and column gets a transaction field
      for (let x = 0; x < this.rowCount; x++) {
          for (let y = 0; y < this.columnCount; y++) {
              if (this.unplacedTransactionFields.length > 0) {
                  const toPlace = this.unplacedTransactionFields.splice(0, 1)[0];
                  toPlace.xPosition = x;
                  toPlace.yPosition = y;
                  toPlace.isPlaced = true;
                  this.placedTransactionFields.push(toPlace);
                  this.getColumnDetails(x, y).fieldToRender = toPlace;
              }
          }
      }
  }

  public fieldsThatNeedToCopyToNextTransaction(): string[] {
    const results: string[] = [];
    this.placedTransactionFields.forEach((x) => {
      if (x.persistDataAfterSave) {
        results.push(x.id);
      }
    });
    return results;
  }

  public resetPlacement(): void {
      this.placedTransactionFields.forEach(x => {
          x.isPlaced = false;
          x.xPosition = null;
          x.yPosition = null;
          x.yPosition = null;
          x.rowSpan = 0;
          x.columnSpan = 0;
          if (x.isLabel || x.isLine) {
              // dont pop this back in
              return;
          }
          this.unplacedTransactionFields.push(x);
      });
      this.placedTransactionFields.length = 0;
      this.constructColumnsAndRows();
      this.sortUnplacedTransactionFields();
  }



  public removeColumn(): void {
      this.resetPlacement();
      this.columnCount--;
      this.constructColumnsAndRows();
  }
  public addColumn(): void {
      this.resetPlacement();
      this.columnCount++;
      this.constructColumnsAndRows();
  }
  public removeRow(): void {
      this.resetPlacement();
      this.rowCount--;
      this.constructColumnsAndRows();
  }
  public addRow(): void {
      this.resetPlacement();
      this.rowCount++;
      this.constructColumnsAndRows();
  }
}
