import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appAllowOnlyDigits]'
})
export class AllowOnlyDigitsDirective {

  // Allow decimal numbers and negative values
  private regex: RegExp = new RegExp(/^[0-9]*$/);
  // Allow key codes for special events. Reflect :
  // Backspace, tab, end, home

  // tslint:disable-next-line:max-line-length
  private specialKeys: Array<string> = [ 'Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'Left', 'ArrowRight', 'Right', 'ArrowUp', 'Up', 'ArrowDown', 'Down', 'Delete', 'Del', 'Backspace', 'Copy' ];

  constructor(private el: ElementRef) { }

  @HostListener('keydown', [ '$event' ])
  onKeyDown(event: KeyboardEvent) {
    // Allow Backspace, tab, end, home keys, arrows and deletes
    if (this.specialKeys.indexOf(event.key) !== -1) {
      return;
    }
    // check for the complete string being highlighted
    if(this.el.nativeElement.selectionStart === 0 && this.el.nativeElement.selectionEnd === this.el.nativeElement.value.length)
    {
      this.el.nativeElement.selectionStart = 0;
      this.el.nativeElement.selectionEnd = 0;
      this.el.nativeElement.value = "";
    }
    const current: string = this.el.nativeElement.value;
    const next: string = ( current.length === 0) ? event.key : current.slice(0, this.el.nativeElement.selectionStart) + event.key + current.slice(this.el.nativeElement.selectionEnd); 
    if (next && !String(next).match(this.regex)) {
      event.preventDefault();
    }
  }

}
