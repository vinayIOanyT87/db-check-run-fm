import { Directive, OnInit, OnChanges, Input, ElementRef, Renderer2, SimpleChanges } from '@angular/core';

@Directive({
  selector: '[appFocusDirective]'
})
export class FocusDirective implements OnInit, OnChanges {
  @Input() appFocusDirective: boolean;
  @Input() nonDefaultIDName: string;
  constructor(private hostElement: ElementRef) { }

  ngOnInit() {
    this.focus();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.focus();
  }

  private focus(): void {
    if (this.appFocusDirective && !(this.nonDefaultIDName === null || this.nonDefaultIDName === undefined)) {
      const inputElement = document.getElementById(this.nonDefaultIDName);
      if (inputElement == null) { return; }
      setTimeout(() => { inputElement.focus(); }, 0);
    } else if (this.appFocusDirective) {
      setTimeout(() => { this.hostElement.nativeElement.focus(); }, 0);
    }
  }
}
