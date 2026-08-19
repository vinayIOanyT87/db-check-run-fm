import { Directive, Input, ElementRef, Renderer2, HostListener } from '@angular/core';

@Directive({
  selector: '[appRestricMinMax]'
})
export class RestricMinMaxDirective {

  @Input('restrict_minvalue') restrict_minvalue: number;
  @Input('restrict_maxvalue') restrict_maxvalue: number;

  constructor(private el: ElementRef, private renderer: Renderer2) {
  }

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    const currentValue: string = this.el.nativeElement.value;
    // tslint:disable-next-line:max-line-length
    const nextValue: string = ( currentValue.length === 0) ? event.key : currentValue.slice(0, this.el.nativeElement.selectionStart) + event.key + currentValue.slice(this.el.nativeElement.selectionEnd);

    if ( !this.isWithinMin( currentValue, nextValue ) ) {
          event.preventDefault();
    }

    if ( !this.isWithinMax(  nextValue ) ) {
        event.preventDefault();
        return false;
    }
  }

  isWithinMin( currentValue, nextValue ) {
    if ( this.restrict_minvalue && nextValue != '') {
      if (  Number( currentValue ) >= this.restrict_minvalue && Number( nextValue ) < this.restrict_minvalue) {
        return false;
      }
    }
    return true;
  }

  isWithinMax( nextValue ) {
    if ( this.restrict_maxvalue ) {
      if ( Number( nextValue ) > this.restrict_maxvalue) {
        return false;
      }
    }
    return true;
  }


  @HostListener('paste', ['$event'])
  onPaste(event) {
    const currentValue: string = this.el.nativeElement.value;
    let nextValue = '';

    event.preventDefault();
    let content = null;
    if (event.clipboardData) {
      content = (event.originalEvent || event).clipboardData.getData('text/plain');
      // check if all we are going to paste is numeric
      if ( !isNaN(parseFloat(content)) && isFinite(content) ) {
        // tslint:disable-next-line:max-line-length
        nextValue = ( currentValue.length === 0) ? content : currentValue.slice(0, this.el.nativeElement.selectionStart) + content + currentValue.slice(this.el.nativeElement.selectionEnd);
        console.log( nextValue);
        if ( this.isWithinMin( currentValue, nextValue ) && this.isWithinMax( nextValue ) ) {
          document.execCommand('insertText', false, content);
        }
      }

    } else if ((<any>window).clipboardData) {
      content = (<any>window).clipboardData.getData('Text');

      // check if all we are going to paste is numeric
      if ( !isNaN(parseFloat(content)) && isFinite(content) ) {
        // tslint:disable-next-line:max-line-length
        nextValue = ( currentValue.length === 0) ? content : currentValue.slice(0, this.el.nativeElement.selectionStart) + content + currentValue.slice(this.el.nativeElement.selectionEnd);
        console.log( nextValue);
        if ( this.isWithinMin( currentValue, nextValue ) && this.isWithinMax( nextValue ) ) {
          document.execCommand('insertText', false, content);
        }
      }
      (<any>document).selection.createRange().pasteHTML(content);
    }
  }
}
