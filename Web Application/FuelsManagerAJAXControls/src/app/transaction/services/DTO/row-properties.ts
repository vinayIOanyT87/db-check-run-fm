
import { TransactionAliasPlacementInformation } from '../../../fm-core/DTO/transaction-alias-placement-information';

export class RowProperties {
    public constructor(init?: Partial<RowProperties>) {
        Object.assign(this, init);
    }

    public rowIndex = 0;
    public columns: ColumnProperties[] = [];

    updateInternalCounters(): TransactionAliasPlacementInformation[] {
        const results = this.updateColumnShown();
        this.columns.forEach(x => {
          if (x.fieldToRender) {
            if (x.fieldToRender.columnSpan === null || x.fieldToRender.columnSpan === undefined) {
              x.fieldToRender.columnSpan = 0;
            }
            if ((x.fieldToRender.rowSpan === null || x.fieldToRender.rowSpan === null)) {
              x.fieldToRender.rowSpan = 0;
            }
            x.rowHeightInEM = (x.fieldToRender.rowSpan * 2) + 2;
          } else {
            x.rowHeightInEM = 2;
          }
        });
        return results;
    }

    private updateColumnShown(): TransactionAliasPlacementInformation[] {
        const fieldsThatNeedToBeRemoved: TransactionAliasPlacementInformation[] = [];
        let numberToRemove = 0;
        const columnsInOrder = this.columns.sort((x, y) => {
            return x.columnIndex - y.columnIndex;
        });
        columnsInOrder.forEach(x => {
            if (numberToRemove > 0) {
                if (!(x.fieldToRender === null || x.fieldToRender === undefined)) { fieldsThatNeedToBeRemoved.push(x.fieldToRender); }
                x.hide = true;
                numberToRemove--;
            } else {
                if (!(x.fieldToRender === null || x.fieldToRender === undefined)) {
                    if (x.fieldToRender.columnSpan > 0) {
                        numberToRemove = x.fieldToRender.columnSpan;
                    }
                }
                x.hide = false;
            }
        });
        this.balanceColumnWidths();
        return fieldsThatNeedToBeRemoved;
    }

    private balanceColumnWidths(): void {
        // x = (z * y) / e
        const totalWidthPercentage = 100; // y
        let totalWidthForColumns = 0; // e
        this.columns.forEach(x => {
            totalWidthForColumns += 1;
        });
        // x.columnWidth z
        this.columns.forEach(x => {
            x.columnComputedWidthPercentage = totalWidthPercentage / totalWidthForColumns;
            if (x.fieldToRender) {
                x.columnComputedWidthPercentage = (x.fieldToRender.columnSpan + 1) * x.columnComputedWidthPercentage;
            }
        });
    }
}

export class ColumnProperties {
    public constructor(init?: Partial<ColumnProperties>) {
        Object.assign(this, init);
    }

    public columnIndex: number;
    public rowIndex: number;
    public columnWidth = 1;
    public columnComputedWidthPercentage = 300;
    public rowHeightInEM = 2;
    public hide = false;
    public fieldToRender: TransactionAliasPlacementInformation = null;
}
