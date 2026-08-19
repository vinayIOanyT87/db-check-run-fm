import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-fm-loading-screen',
  templateUrl: './fm-loading-screen.component.html',
  styleUrls: ['./fm-loading-screen.component.css']
})
export class FMLoadingScreenComponent {
  @Input() public width = 120;

  public show = true;
  getBorder(loadingWidth: number): number {
      return Math.ceil(loadingWidth / 7);
  }

}
