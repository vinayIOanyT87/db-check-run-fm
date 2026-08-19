import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appFieldHost]'
})
export class FieldHostDirective {
    constructor(public viewContainerRef: ViewContainerRef) { }
}
