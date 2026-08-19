import { Directive, HostListener, ApplicationRef } from '@angular/core';

@Directive({
  selector: '[appTriggerChangeDetectionOnKeyUp]'
})
/**
 * Needed for some of our observable/subscriptions actions
 */
export class TriggerChangeDetectionOnKeyUpDirective {
  constructor(private _appRef: ApplicationRef) { }
  @HostListener('keyup', ['$event'])
  public handleKeyboardEvent(event: KeyboardEvent): void {
    setTimeout(() => {
      this._appRef.tick();
    }, 0);
  }
  @HostListener('blur', ['$event'])
  public handleBlur(event: any): void {
    setTimeout(() => {
      this._appRef.tick();
    }, 0);
  }
  @HostListener('focus', ['$event'])
  public handleFocus(event: any): void {
    setTimeout(() => {
      this._appRef.tick();
    }, 0);
  }
}

// readonly enterKeyCode: number = 13;
// @HostListener('keypress', ['$event'])
// public handleKeyboardEvent(event: KeyboardEvent): void {
//     if (event.key === 'Enter') {
//         event.preventDefault();
//         event.stopPropagation();
