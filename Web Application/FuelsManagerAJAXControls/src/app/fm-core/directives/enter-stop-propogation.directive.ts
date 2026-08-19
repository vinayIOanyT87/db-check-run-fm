import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[appEnterStopPropogation]'
})
export class EnterStopPropogationDirective {
  readonly enterKeyCode: number = 13;
  @HostListener('keypress', ['$event'])
  public handleKeyboardEvent(event: KeyboardEvent): void {
      if (event.key === 'Enter') {
          event.preventDefault();
          event.stopPropagation();
      }

  }
}
