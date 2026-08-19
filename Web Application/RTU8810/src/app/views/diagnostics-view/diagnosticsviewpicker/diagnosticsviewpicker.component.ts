import { Component, OnInit, TemplateRef, ViewChild, ViewChildren } from '@angular/core';
import { RtuconfigurationService, Diagview, IRTUConfiguration, IPoint } from 'src/app/services/rtuconfiguration.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { Subscription } from 'rxjs';

class tank {
  pointId : number;
  tankId : number;
  label: string;
  point: IPoint;

  constructor(pointId, tankId, label, point){
    this.pointId = pointId;
    this.tankId = tankId;
    this.label = label;
    this.point = point;
  }
}

@Component({
  selector: 'app-diagnosticsviewpicker',
  templateUrl: './diagnosticsviewpicker.component.html',
  styleUrls: ['./diagnosticsviewpicker.component.css']
})


export class DiagnosticsviewpickerComponent implements OnInit {
  rtuConfigurationSubscription : Subscription;
  diagnosticViewSubscription : Subscription;
  rtuconfiguration: IRTUConfiguration;
  views: Diagview[];
  closeResult: string;
  isLoaded: boolean;
  uniqueNameWarning = false;
  currentView:Diagview;
  activeViewSrc: string= './assets/activesaveddiagnostic.png';
  inactiveViewSrc: string  = './assets/saveddiagnostic.png';
  viewType: number = 0;
  viewName: string = '';
  copyViewName: string = '';
  activeTanks: tank[];
 
  @ViewChild('modalNewView', { static: true }) public modalNewView: TemplateRef<any>;
  @ViewChild('modalCopyView', { static: true }) public modalCopyView: TemplateRef<any>;
  @ViewChild('modalDeleteView', { static: true }) public modalDeleteView: TemplateRef<any>;
  @ViewChild('modalSelectView', { static: false }) public modalSelectView: TemplateRef<any>;


  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false
  };

  constructor(
    private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService) {
      this.isLoaded= false;
      this.views = [];
     }
    

  ngOnInit() {
    this.getRtuConfiguration();
    this.subscribeActiveDiagnosticView();
  }

  ngOnDestroy(){
    this.rtuConfigurationSubscription.unsubscribe();
    this.diagnosticViewSubscription.unsubscribe();
  }

  //modal for entering the view name
  newViewButton() {
    this.viewType = 0;
    this.modalRef = this._modalService.show( this.modalNewView , this.modalConfig);
  }

  //load available diagnostic views
  getRtuConfiguration(): void {
    let instance = this;
  
    this.rtuConfigurationSubscription = this._rtuConfiguration.get().subscribe( data => {
      if ( data.RTUConfiguration ) {
        instance.rtuconfiguration = data.RTUConfiguration;
        instance.views = data.RTUConfiguration.diagViews;
        instance.getActiveTanks();
      }
      else {
        instance.views = [];
      }
    });
  }

  compareTanks(a: tank, b: tank) {
    if (a.label < b.label)
      return -1;
    if (a.label > b.label)
      return 1;
    return 0;
  }

  getActiveTanks(){
    const instance = this;
    let tankLabelIdentifier = -1;
    let tankVisibleIdentifer = -1;
    let tankId = 0;
    instance.activeTanks = [];
    let numberOfTanks = 20;
    let moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
    let numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
    if(numberOfTanksIdentifier){
      numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].value);
    }

    instance.rtuconfiguration.points.forEach(function(point, index){

      if(tankId >= numberOfTanks){
        return;
      }

      if(point.name === 'Tank'){
        if(tankLabelIdentifier === -1
        || tankVisibleIdentifer === -1){
          let labelIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'Label');
          if(labelIdentifier){
            tankLabelIdentifier = parseInt(labelIdentifier);
          }

          let visibleIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'TankVisible');
          if(visibleIdentifier){
            tankVisibleIdentifer = parseInt(visibleIdentifier);
          }
        }

        if(tankLabelIdentifier === -1
        || tankVisibleIdentifer === -1){
            return;
        }

        let labelParameter = point.pointConfiguration[tankLabelIdentifier + index];
        let tankVisibleParameter = point.pointConfiguration[tankVisibleIdentifer + index];
        let visible = tankVisibleParameter.pendingValue;

        if(visible === '2'){
          let newTank = new tank(index, tankId++, labelParameter.pendingValue, point);
          instance.activeTanks.push(newTank);
        }
      }
    });

    instance.activeTanks = instance.activeTanks.sort(instance.compareTanks);

  }

  subscribeActiveDiagnosticView(): void {
    this.diagnosticViewSubscription = this._rtuConfiguration.retrieveDiagnosticView().subscribe( data => {
      if ( data ) {
        this.currentView = data;
          //enroll realtime identifiers for new view
      }
      else {
        this.currentView = {id: 'Could not retrieve active view', parameters:[], filterCollection: {dataType:0, filters:[]}};
      }
    });
  }

  //create new view with no data
  add(name: string, viewType: number): void {
    name = name.trim();
    if (!name) { return; }
    var copy = this.views.find(x => x.id === name);
    if (copy != undefined)
    {
      this.uniqueNameWarning = true;
      setTimeout(()=>{this.uniqueNameWarning = false;},2500);
    }
    else {
      this._rtuConfiguration.addView({ id: name, parameters: [], filterCollection: {dataType:viewType, filters:[]} } as Diagview)
      this.modalRef.hide();
    }
  }

  //delete view with given id. need to update to use unique identifier??
  deletePrompt(id:string): void {
    this.modalRef = this._modalService.show( this.modalDeleteView , this.modalConfig);
  }

  //delete view with given id. need to update to use unique identifier??
  delete(id:string): void {
    this._rtuConfiguration.delView(id);
    this.modalRef.hide();
  }

  copyViewButton(id:string)
  {
    //this._rtuConfiguration.setActiveDiagnosticView(id);
    this.modalRef = this._modalService.show( this.modalCopyView , this.modalConfig);
  }

  copyActive(name:string)
  {
    name = name.trim();
    if (!name) { return; }
    var copy = this.views.find(x => x.id === name);
    if (copy != undefined)
    {
      this.uniqueNameWarning = true;
      setTimeout(()=>{this.uniqueNameWarning = false;},2500);
    }
    else
    {
      var copiedView: Diagview = JSON.parse(JSON.stringify(this.currentView));
      copiedView.id = name;
      this._rtuConfiguration.addView( copiedView );
      this.modalRef.hide();
    }
  }

  //set the active view so that the tag viewer can load it
  setActiveDiagnosticView(id:string){
    this._rtuConfiguration.setActiveDiagnosticView(id);
  }
}
