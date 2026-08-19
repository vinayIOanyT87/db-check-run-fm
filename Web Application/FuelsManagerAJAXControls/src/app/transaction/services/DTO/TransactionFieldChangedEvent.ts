
export class TransactionFieldChangedEvent {
  source = '';
  list:any = null;
  field = '';
  value = '';
  initialData = false;

  constructor(obj?: {value?: string, field?: string, source?: string, list?: string[], initialData?: boolean }) {
    if (!(obj === null || obj === undefined) && !(obj.source === null || obj.source === undefined)) {
      this.source = obj.source;
    }
    if (!(obj === null || obj === undefined) && !(obj.value === null || obj.value === undefined)) {
      this.value = obj.value;
    }
    if (!(obj === null || obj === undefined) && !(obj.field === null || obj.field === undefined)) {
      this.field = obj.field;
    }
    if (!(obj === null || obj === undefined) && !(obj.list === null || obj.list === undefined)) {
      this.list = obj.list;
    }
    if (!(obj === null || obj === undefined) && !(obj.initialData === null || obj.initialData === undefined)) {
      this.initialData = obj.initialData;
    }
  }
}
