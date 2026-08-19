import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
    selector: 'app-transaction-label',
    template: `<h4 style='white-space:nowrap; overflow:hidden;'>{{label}}</h4>`,
    styles: [``]
})
export class TransactionLabelComponent {
    public label = '';
    constructor() { }
}
