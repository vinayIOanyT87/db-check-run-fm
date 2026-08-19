import { Injectable } from '@angular/core';
import { NgbModal, ModalDismissReasons, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { FMLoadingScreenComponent } from '../webparts/fm-loading-screen/fm-loading-screen.component';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { Overlay } from '@angular/cdk/overlay';

@Injectable({
  providedIn: 'root'
})
export class LoadingScreenService {
  private isLoading = false;
  private currentModal: MatDialogRef<any> = null;
  private minimumTimeToShow = 0;
  private minimumTimeToShowHasPassed = false;
  private needToShowModal = false;
  // constructor(private modalService: NgbModal) {
  // }
  constructor(private dialog: MatDialog, private overlay: Overlay) {

  }

  /**
   * shows loading screen
   * @param minimumTimeShowing minimum time in milliseconds to show loading dialog, leave out to return immidiately
   */
  showLoadingScreen(minimumTimeShowing: number = 0): void {
      if (!this.isLoading) {
          // setup
          this.isLoading = true;
          this.minimumTimeToShowHasPassed = true;
          this.needToShowModal = true;
          this.minimumTimeToShow = minimumTimeShowing;

          // lets setup the minimum time to show portion
          if (this.minimumTimeToShow > 0) {
              this.needToShowModal = false;
              this.minimumTimeToShowHasPassed = false;
              const that = this; // we lose the this reference
              window.setTimeout(function () {
                  that.minimumTimeToShowHasPassed = true;
                  if (that.needToShowModal === true) {
                      that.hideScreen();
                  }
              }, this.minimumTimeToShow);
          }

          // open up modal
          // this.currentModal = this.modalService.open(FMLoadingScreenComponent,
          //     { size: 'sm', centered: true, backdrop: 'static' });
          this.currentModal = this.dialog.open(FMLoadingScreenComponent,
            {
              role: 'alertdialog',
              hasBackdrop: true,
              width: '250px',
              height: '220px',
              disableClose: true,
              scrollStrategy: this.overlay.scrollStrategies.noop()
             });
      }
  }

  hideLoadingScreen(): void {
      if (this.isLoading) {
          this.needToShowModal = true;
          if (this.minimumTimeToShowHasPassed) {
              this.hideScreen();
          }
      }
  }

  private hideScreen(): void {
      this.isLoading = false;
      this.currentModal.close();
  }
}
