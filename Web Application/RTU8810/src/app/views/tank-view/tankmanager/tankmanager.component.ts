import { Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy, ElementRef, TemplateRef, ViewChild } from '@angular/core';
import { RtuconfigurationService, IRTUConfiguration, IPoint, IPointData } from 'src/app/services/rtuconfiguration.service';
import { IParameter, configClass, allProtocols } from 'src/app/services/availablemodules.service';
import * as saveAs from 'node_modules/file-saver';
import { FindValueSubscriber } from 'rxjs/internal/operators/find';
import { Subscription } from 'rxjs';
// import { instantiateRootComponent } from '@angular/core/src/render3/instructions';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { ScrollingModule } from '@angular/cdk/scrolling';


const TankActiveImage = "./assets/Tank Active.png";
const TankInactiveImage = "./assets/Tank Inactive.png";
const TankSelectedImage = './assets/Tank Selected.png';

interface ICell {
  tabIndex: number;
  object: any;
  disableOverride: boolean;
}

interface ITab {
  tabName: string;
  sections: ISection[];
  cells: ICell[];
}

interface ISection {
  sectionName: string;
  sections: ISection[];
  parameters: IParameter[];
}

class tank {
  pointId: number;
  tankId: number;
  label: string;
  tanklistimg: string;
  activationimg: string;
  point: IPoint;

  constructor(pointId, tankId, label, point) {
    this.pointId = pointId;
    this.tankId = tankId;
    this.label = label;
    this.point = point;
    this.tanklistimg = TankActiveImage;
    this.activationimg;
  }
}


@Component({
  selector: 'app-tankmanager',
  templateUrl: './tankmanager.component.html',
  styleUrls: ['./tankmanager.component.css'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.Default
})
export class TankmanagerComponent implements OnInit {
  configurationColumns = 3;
  selectedTank: tank;
  selectedTanks: tank[] = [];
  batchEditTank: tank;
  tankViewTabs: ITab[];
  tankParameters: IParameter[];
  tanksToChangeVisibility: IPointData[];
  activeTanks: tank[];
  filteredActiveTanks: tank[];
  filteredActiveTankRows: any[];
  inactiveTanks: tank[];
//  activationTanks: tank[];
  filteredActivationTanks: tank[];
  activationTankRows: any[];
  tanksToActivate: number[];
  tankVisibleIdentifier: number;
  labelIdentifier: number;
  searchTankToggle = false;
  searchTankString = '';
  searchConfigurationToggle = false;
  searchConfigurationString = '';
  searchActivationToggle = false;
  searchActivationString = '';
  isLoaded: boolean;
  tankActivationMode: string;
  tankActivationApplyButtonText: string;
  rtuconfiguration: IRTUConfiguration;
  activationActive = false;
  setActivateMultiLabel: string = '';
  autoIncrementParameterName: string;
  rtuconfigurationSubscription: Subscription;
  dragSelectActive = false;
  disableDeviceTypeDropDown = false;
  disableDeviceCommandDropDown = false;
  disableNmsDeviceCommandDropDown = false;
  showPointIndexes = false;




  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  @ViewChild('activateLabelModalBackground', { static: false }) activateLabelModalBackground: ElementRef;
  @ViewChild('batchRenameLabel', { static: false }) batchRenameLabel: ElementRef;
  @ViewChild('modalVerifyApplytoRTUAction', { static: true }) public modalVerifyApplytoRTUAction: TemplateRef<any>;
  @ViewChild('modalVerifyCancelAction', { static: true }) public modalVerifyCancelAction: TemplateRef<any>;
  @ViewChild('modalVerifyCancelActivation', { static: true }) public modalVerifyCancelActivation: TemplateRef<any>;
  @ViewChild('modalNothingtowritetoRTUAction', { static: true }) public modalNothingtowritetoRTUAction: TemplateRef<any>;
  @ViewChild('modalAutoIncrementValue', { static: true }) public modalAutoIncrementValue: TemplateRef<any>;



  constructor(
    private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService) {
    this.isLoaded = false;
  }

  ngOnInit() {
    this.showPointIndexes = (localStorage.getItem('showPointIndexes') === 'true') ? true : false;
    this.getRTUConfiguration();
  }

  ngOnDestroy() {
    if (this._rtuConfiguration) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.tankParameters);
    }
    if (this.rtuconfigurationSubscription) {
      this.rtuconfigurationSubscription.unsubscribe();
    }
  }

  compareTanks(a: tank, b: tank) {
    if (a.label < b.label)
      return -1;
    if (a.label > b.label)
      return 1;
    return 0;
  }

  getActiveTankClass(i: number) {
    if ((i - 1) % 4 === 0) {
      return "tank-border";
    }
    else {
      return "tank-plain";
    }
  }


  getActivationTankClass(i: number) {
    if ((i - 1) % 4 === 0) {
      return "tank-border";
    }
    else if ((i - 3) % 4 === 0) {
      return "tank-margin-right";
    }
    else if ((i - 4) % 4 === 0) {
      return "tank-margin-left";
    }
    else {
      return "tank-plain";
    }
  }

  togglePointIdx()
  {
    this.showPointIndexes = !this.showPointIndexes;
    localStorage.setItem('showPointIndexes',this.showPointIndexes.toString());
    this.getActiveInactiveTanks();
    this.getFilteredActiveTanks();
    this.getTankViewTabs();
  }

  getActiveInactiveTanks() {
    if (this._rtuConfiguration
      && this.tankParameters) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.tankParameters);
    }

    this.tankParameters = [];
    let activeTanks = [];
    let inactiveTanks = [];
    this.labelIdentifier = -1;
    this.tankVisibleIdentifier = -1;
    let instance = this;
    let tankId = 0;


    if (this.rtuconfiguration != null
      && this.rtuconfiguration.points != null) {
      let numberOfTanks = 20;
      let moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
      let numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
      if (numberOfTanksIdentifier) {
        numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].pendingValue);
        instance.tankParameters.push(moduleConfiguration[numberOfTanksIdentifier]);
      }

      this.rtuconfiguration.points.forEach(function (point, index) {

        if (tankId >= numberOfTanks) {
          return;
        }

        if (point.name === 'Tank') {
          if (instance.labelIdentifier === -1
          || instance.tankVisibleIdentifier === -1) {
            let labelIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'Label');
            if (labelIdentifier) {
              instance.labelIdentifier = parseInt(labelIdentifier);
            }

            let visibleIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'TankVisible');
            if (visibleIdentifier) {
              instance.tankVisibleIdentifier = parseInt(visibleIdentifier);
            }
          }

          if (instance.labelIdentifier === -1
            || instance.tankVisibleIdentifier === -1) {
            return;
          }

          let labelParameter = point.pointConfiguration[instance.labelIdentifier + index];
          let newTank = new tank(index, tankId++, labelParameter.pendingValue, point);
          let tankVisibleParameter = point.pointConfiguration[instance.tankVisibleIdentifier + index];
          let visible = tankVisibleParameter.pendingValue;

          instance.tankParameters.push(labelParameter);
          instance.tankParameters.push(tankVisibleParameter);

          let existingTank;

          if (instance.activeTanks) {
            existingTank = instance.activeTanks.find(s => s.pointId === newTank.pointId);
            if (!existingTank
              && instance.inactiveTanks) {
              existingTank = instance.inactiveTanks.find(s => s.pointId === newTank.pointId);
            }
          }

          // preserve prior images
          if (existingTank) {
            newTank.tanklistimg = existingTank.tanklistimg;
            newTank.activationimg = existingTank.activationimg;
          }

          if (visible === '2') {
            activeTanks.push(newTank);
            if (instance.selectedTank
              && instance.selectedTank.tankId === newTank.tankId) {
              instance.selectedTank = newTank;
            }

          }
          else {
            if (instance.selectedTank
              && instance.selectedTank.tankId === newTank.tankId) {
              instance.selectedTank = null;
            }

            // inactive tank must not be selected when activated.
            newTank.tanklistimg = TankActiveImage;
            inactiveTanks.push(newTank);
          }
        }
      });
    }

    instance.activeTanks = activeTanks.sort(instance.compareTanks);
    instance.inactiveTanks = inactiveTanks.sort(instance.compareTanks);

    if (instance.selectedTank != null) {
      Object.keys(instance.selectedTank.point.pointConfiguration).map(s => instance.selectedTank.point.pointConfiguration[s]).forEach(function (parameter) {
        instance.tankParameters.push(parameter);
      });
    }

    if (instance._rtuConfiguration
      && instance.tankParameters) {
      instance._rtuConfiguration.subscribeRealtimeParameters(this.tankParameters);
    }
  }

  getFilteredActiveTanks() {
    let filteredActiveTanks = [];
    let instance = this;
    let upperCaseSearchString = this.searchTankString.toUpperCase();
    this.activeTanks.forEach(function (tank) {
      if (instance.showPointIndexes){
        const indexSuffix =  " (" + (tank.pointId+1).toString().padStart(3, '0') +")";
        if(tank.label.indexOf(indexSuffix) === -1){
          tank.label += indexSuffix;
        }
      }
      if (!instance.searchTankString
      || instance.searchTankString === ''
      || (instance.searchTankString !== ''
      && tank.label.toUpperCase().indexOf(upperCaseSearchString) !== -1)) {
        filteredActiveTanks.push(tank);
      }
      else if (tank === instance.selectedTank) {
        tank.tanklistimg = TankActiveImage;
        instance.selectedTank = null;
      }
    });
    
    filteredActiveTanks = filteredActiveTanks.sort(this.compareTanks);

    let change = false;
    let activeTankRows = [];
    let activeTankRow;
    if(filteredActiveTanks.length > 0){
      for(let i=0;i < filteredActiveTanks.length;i++){
        if(i % 4 === 0){
          activeTankRow = [];
          activeTankRows.push(activeTankRow);
        }
        activeTankRow.push(filteredActiveTanks[i]);
        if(!change
        && (!instance.filteredActiveTanks
        || filteredActiveTanks.length != instance.filteredActiveTanks.length 
        || filteredActiveTanks[i].tankId != instance.filteredActiveTanks[i].tankId
        || filteredActiveTanks[i].label != instance.filteredActiveTanks[i].label
        || filteredActiveTanks[i].tanklistimg != instance.filteredActiveTanks[i].tanklistimg)){
            change = true;
        } 
      }
    }
    else if(!instance.filteredActiveTanks
    || instance.filteredActiveTanks.length > 0){
      change = true;
    }

    if(change){
      instance.filteredActiveTanks = filteredActiveTanks;
      instance.filteredActiveTankRows = activeTankRows;
    }
  }

  getFilteredActivationTanks() {
    let instance = this;
    let activationTanks = [];
    let filteredActivationTanks = [];
    let upperCaseSearchString = instance.searchActivationString.toUpperCase();

    if (this.tankActivationMode === 'DEACTIVATE TANKS') {
      activationTanks = instance.activeTanks;

      activationTanks.forEach(function (tank) {
        if (!instance.searchActivationString
          || instance.searchActivationString === ''
          || (instance.searchActivationString !== ''
          && tank.label.toUpperCase().indexOf(upperCaseSearchString) != -1)) {
            filteredActivationTanks.push(tank);
        }
      });
  
      filteredActivationTanks = filteredActivationTanks.sort(this.compareTanks);
    }
    else {
      activationTanks = instance.inactiveTanks;

      activationTanks.forEach(function (tank) {
        if (!instance.searchActivationString
          || instance.searchActivationString === ''
          || (instance.searchActivationString !== ''
          && ('Tank ' + ('000' + (tank.tankId + 1)).slice(-3)).toUpperCase().indexOf(upperCaseSearchString) != -1)) {
            filteredActivationTanks.push(tank);
        }
      });
  
      filteredActivationTanks = filteredActivationTanks.sort(this.comparePointId);
    }

    let change = false;
    let activationTankRows = [];
    let activationTankRow;
    if(filteredActivationTanks.length > 0){
      for(let i=0;i < filteredActivationTanks.length;i++){
        if(i % 8 === 0){
          activationTankRow = [];
          activationTankRows.push(activationTankRow);
        }
        activationTankRow.push(filteredActivationTanks[i]);

        if (!change
        && (!instance.filteredActivationTanks
        || filteredActivationTanks.length != instance.filteredActivationTanks.length
        || filteredActivationTanks[i].tankId != instance.filteredActivationTanks[i].tankId
        || filteredActivationTanks[i].label != instance.filteredActivationTanks[i].label
        || filteredActivationTanks[i].activationimg != instance.filteredActivationTanks[i].activationimg)) {
          change = true;
        }
      }
    }
    else if(!instance.filteredActivationTanks
    || instance.filteredActivationTanks.length > 0){
      change = true;
    }

    if(change){
      instance.filteredActivationTanks = filteredActivationTanks;
      instance.activationTankRows = activationTankRows;
    }
  }



  getRTUConfiguration(): any {
    let instance = this;

    this.rtuconfigurationSubscription = this._rtuConfiguration.get().subscribe(data => {
      if (data.RTUConfiguration) {
        instance.rtuconfiguration = data.RTUConfiguration;
      } else {
        instance.rtuconfiguration = null;
      }

      var selectedTank = instance.selectedTank;

      instance.getActiveInactiveTanks();
      instance.getFilteredActiveTanks();
      instance.getFilteredActivationTanks();

      // if the selected tank changes or the selectedTank label changes update the tabs
      if ((selectedTank && !instance.selectedTank)
        || (selectedTank && instance.selectedTank && selectedTank.label != instance.selectedTank.label)
        || !instance.tankViewTabs
        || instance.tankViewTabs.length === 0) {
        instance.getTankViewTabs();
      }

      if (instance.tankActivationMode !== 'ACTIVATE TANKS') {
         this.activationActive = false;
      }
    });
  }

  onParameterChange(parameter: IParameter) {
    if (this.selectedTanks.length > 1) {
      let instance = this;
      let tanks = this.selectedTanks;
      tanks.forEach(function (tank, index) {
        let matchingKey = Object.keys(tank.point.pointConfiguration).find(key => tank.point.pointConfiguration[key].parameter === parameter.parameter);
        if (matchingKey) {
          let originalPendingValue = tank.point.pointConfiguration[matchingKey].pendingValue;
          tank.point.pointConfiguration[matchingKey].pendingValue = parameter.pendingValue;
            instance.checkForIncrementGlobalPendingChanges(tank.point.pointConfiguration[matchingKey], originalPendingValue);
        }
      });
    }




    if (parameter.parameter === 'Label'
      || parameter.parameter === 'TankVisible') {
      let instance = this;
      this.getActiveInactiveTanks();
      this.getFilteredActiveTanks();
      this.getFilteredActivationTanks();
      this.getTankViewTabs();

      if (parameter.parameter === 'Label') {
        setTimeout(() => { this.setnexttabbeditem() }, 500);
      }

      if (parameter.parameter === 'Label') {
        setTimeout(() => { this.setnexttabbeditem() }, 500);
      }

    }
    else if (parameter.parameter === "DeviceType" || parameter.parameter === "Channel" || parameter.parameter === "Module") {
      let instance = this;
      instance.populateDeviceDropdowns(instance, true);
      instance.getTankViewTabs();
      setTimeout(() => {
        const input1 = document.getElementById(parameter.parameter);
        input1.focus();
      }, 500);
    }
    else if (parameter.configClass === configClass.COMMAND) {
      this._rtuConfiguration.applyCommandToRTU(parameter);
    }
  }

  setnexttabbeditem() {
    const input1 = document.getElementById('Label');
    input1.focus();
  }

  getTankViewSection(subSections: ISection[], sections: string[], sectionIndex: number) {
    let section = subSections.find(s => s.sectionName === sections[sectionIndex]);
    if (!section) {
      section = { 'sectionName': sections[sectionIndex], 'sections': [], 'parameters': [] };
      subSections.push(section);
    }

    if (sectionIndex < sections.length - 1) {
      section = this.getTankViewSection(section.sections, sections, sectionIndex + 1);
    }

    return section;
  }

  getTankViewMultiSection(subSections: ISection[], sections: string[], sectionName: string) {
    if (sectionName == '')
      sectionName = "Label (Batch Edit)";
    let section = subSections.find(s => s.sectionName === sectionName);
    if (!section) {
      section = { 'sectionName': sectionName, 'sections': [], 'parameters': [] };
      subSections.push(section);
    }

    // if (sectionIndex < sections.length - 1) {
    //   section = this.getTankViewSection(section.sections, sections, sectionIndex + 1);
    // }

    return section;
  }

  isParameterCell(cell: ICell) {
    let cellType = typeof cell.object;
    return (cellType === 'string') ? false : true;
  }

  isSpacerCell(cell: ICell) {
    if (cell.object == "SPACER")
      return true;
    return false;
  }

  getTankViewTabCells(tab: ITab, sections: ISection[]) {
    let instance = this;
    let upperCaseSearchString = this.searchConfigurationString.toUpperCase();
    sections.forEach(function (section) {
      let cell = { tabIndex: 0, object: section.sectionName, disableOverride: false };
      tab.cells.push(cell);
      section.parameters.forEach(function (parameter) {
        if (!instance.searchConfigurationString
          || instance.searchConfigurationString === ''
          || (instance.searchConfigurationString !== ''
            && parameter.parameter.toUpperCase().indexOf(upperCaseSearchString) != -1)) {
          let cell = { tabIndex: 0, object: parameter, disableOverride: false };
          tab.cells.push(cell);
        }
      });
      instance.getTankViewTabCells(tab, section.sections);
    });
  }

  getTankViewTabs() {
    let instance = this;
    if (!instance.tankViewTabs) {
      instance.tankViewTabs = [];
    }
    else {
      instance.tankViewTabs.forEach(function (tab) {
        tab.cells = [];
        tab.sections = [];
      });
    }

    if (!instance.selectedTank
      && instance.activeTanks.length === 0
      && instance.inactiveTanks.length === 0) {
      return;
    }

    if (this.selectedTanks.length > 1)
      this.populateTabsBatchEdit(instance);
    else
      this.populateTabsSingleTank(instance);
  }

  populateTabsSingleTank(instance: any) {
    let tank = instance.selectedTank;
    if (!tank) {
      if (instance.activeTanks.length !== 0) {
        tank = instance.activeTanks[0];
      }
      else {
        tank = instance.inactiveTanks[0];
      }
    }

    Object.keys(tank.point.pointConfiguration).map(s => tank.point.pointConfiguration[s]).forEach(function (parameter) {

      // All tank parameters should have a tab
      if (!parameter.tab
        || parameter.tab === '') {
        return;
      }

      // Parameters with no section, use the tank.label;
      let sections = [];
      if (!parameter.section
        || parameter.section === '') {

        // tank label may be null
        if (tank.label) {
          sections = tank.label.split(',');
        }
        else {
          sections.push('');
        }
      }
      else {
        sections = parameter.section.split(',');
      }

      if (!sections
        || sections.length === 0) {
        return;
      }


      let tab = instance.tankViewTabs.find(s => s.tabName === parameter.tab);

      if (!tab) {
        tab = { 'tabName': parameter.tab, 'sections': [], 'cells': [] };
        instance.tankViewTabs.push(tab);
      }

      const sectionIndex = 0;

      let section = instance.getTankViewSection(tab.sections, sections, sectionIndex);

      if (section) {
        section.parameters.push(parameter);
      }
    });

    //config tab needs to be first. Will need to revisit when alarms are added.
    if (instance.tankViewTabs[0].tabName == "Command" && instance.tankViewTabs.length == 2) { this.tankViewTabs.reverse(); }

    // Get Cells for each tab and then reorder the cells to produce rows based upon number of columns
    if (instance.selectedTank === tank) {
      this.tankViewTabs.forEach(function (tab) {
        instance.getTankViewTabCells(tab, tab.sections);
        let cells = [];
        let numberOfRows = Math.floor((tab.cells.length + instance.configurationColumns - 1) / instance.configurationColumns);
        let numberOfCells = numberOfRows * instance.configurationColumns;
        for (let index = 0; index < numberOfCells; index++) {
          let cellIndex = ((index % instance.configurationColumns) * numberOfRows) + Math.floor(index / instance.configurationColumns);
          if (cellIndex < tab.cells.length) {
            let cell = tab.cells[cellIndex];
            cell.tabIndex = cellIndex;
            if (cell.object.parameter === "DeviceType" && instance.disableDeviceTypeDropDown === true) { cell.disableOverride = true; }
            if (cell.object.parameter === "DeviceCmd" && instance.disableDeviceCommandDropDown === true) { cell.disableOverride = true; }
            if (cell.object.parameter === "NMSDeviceCmd" && instance.disableNmsDeviceCommandDropDown === true) { cell.disableOverride = true; }
            cells.push(cell);
          }
          else {
            cells.push({
              tabIndex: {},
              object: "SPACER",
              disableOverride: false
            });
          }
        }
        tab.cells = cells;
      });
    }
  }

  populateTabsBatchEdit(instance: any) {
    let tanks = instance.selectedTanks;
    this.batchEditTank = JSON.parse(JSON.stringify(tanks[0]));
    let sections = [];

    Object.keys(this.batchEditTank.point.pointConfiguration).map(s => this.batchEditTank.point.pointConfiguration[s]).forEach(function (parameter) {

      // All tank parameters should have a tab
      if (!parameter.tab
        || parameter.tab === '') {
        return;
      }

      // Parameters with no section, use the tank.label;

      if (!parameter.section
        || parameter.section === '') {
        sections[0] = "Tank Label"
      }
      else {
        let matchingSection = sections.find(obj => { return obj == parameter.section })
        if (!matchingSection)
          sections.push(parameter.section);
      }

      if (!sections
        || sections.length === 0) {
        return;
      }


      let tab = instance.tankViewTabs.find(s => s.tabName === parameter.tab);

      if (!tab) {
        tab = { 'tabName': parameter.tab, 'sections': [], 'cells': [] };
        instance.tankViewTabs.push(tab);
      }

      let section = instance.getTankViewMultiSection(tab.sections, sections, parameter.section);

      if (section) {
        var matchedParameter = section.parameters.filter(param => { return param.parameter == parameter.parameter });
        if (!matchedParameter[0]) {
          if (parameter) {
            parameter.opcstartNodeID = -999;
            section.parameters.push(parameter);
          }
        }
        else if (matchedParameter[0].pendingValue != parameter.pendingValue) { parameter.pendingValue = undefined; matchedParameter[0].pendingValue = undefined; }

      }
    });

    tanks.forEach(function (tank) {

      Object.keys(tank.point.pointConfiguration).map(s => tank.point.pointConfiguration[s]).forEach(function (parameter) {

        // All tank parameters should have a tab
        if (!parameter.tab
          || parameter.tab === '') {
          return;
        }

        // Parameters with no section, use the tank.label;

        if (!parameter.section
          || parameter.section === '') {
          sections[0] = "General Tank Config"
        }
        else {
          let matchingSection = sections.find(obj => { return obj == parameter.section })
          if (!matchingSection)
            sections.push(parameter.section);
        }

        if (!sections
          || sections.length === 0) {
          return;
        }


        let tab = instance.tankViewTabs.find(s => s.tabName === parameter.tab);

        if (!tab) {
          tab = { 'tabName': parameter.tab, 'sections': [], 'cells': [] };
          instance.tankViewTabs.push(tab);
        }

        const sectionIndex = 0;

        let section = instance.getTankViewMultiSection(tab.sections, sections, parameter.section);

        if (section) {
          var matchedParameter = section.parameters.filter(param => { return param.parameter == parameter.parameter });
          if (matchedParameter[0].pendingValue != parameter.pendingValue) { matchedParameter[0].pendingValue = ''; }

        }
      });

      //config tab needs to be first. Will need to revisit when alarms are added.
      if (instance.tankViewTabs[0].tabName == "Command" && instance.tankViewTabs.length == 2) { this.tankViewTabs.reverse(); }
    });



    // Get Cells for each tab and then reorder the cells to produce rows based upon number of columns
    
    instance.tankViewTabs.forEach(function (tab) {
      instance.getTankViewTabCells(tab, tab.sections);
      let cells = [];
      let numberOfRows = Math.floor((tab.cells.length + instance.configurationColumns - 1) / instance.configurationColumns);
      let numberOfCells = numberOfRows * instance.configurationColumns;
      for (let index = 0; index < numberOfCells; index++) {
        let cellIndex = ((index % instance.configurationColumns) * numberOfRows) + Math.floor(index / instance.configurationColumns);
        if (cellIndex < tab.cells.length) {
          let cell = tab.cells[cellIndex];
          cell.tabIndex = cellIndex;
          if (cell.object.configClass == configClass.COMMAND) { cell.disableOverride = true; }
          else if (cell.object.parameter === "DeviceType" && instance.disableDeviceTypeDropDown === true) { cell.disableOverride = true; }
          cells.push(cell);
        }
        else {
          cells.push({
            tabIndex: {},
            object: "SPACER",
            disableOverride: false
          });
        }
      }
      tab.cells = cells;
    });
  }

  onTankImgClick(event: any, pointId: number, tankId: number) {
    let instance = this;
    if(instance.activationActive === true)
      return;
    if (event.target.src.indexOf('Active.png') != -1) {

      if (event.ctrlKey) { //ctrl clicked an unselected tank. Add it to selectedTanks
        event.target.src = TankSelectedImage;
        instance.selectedTank = instance.activeTanks.find(s => s.pointId === pointId);
        instance.selectedTanks.push(instance.selectedTank);
        instance.selectedTank.tanklistimg = TankSelectedImage;


      }
      else if (event.shiftKey) {
        if (instance.selectedTank) //a tank is selected. select everything in between.
        {
          event.target.src = TankSelectedImage;
          var startIndex = instance.activeTanks.indexOf(instance.selectedTank);
          var endIndex = instance.activeTanks.indexOf(instance.activeTanks.find(s => s.pointId === pointId));
          if (startIndex > endIndex)
            startIndex = endIndex + (endIndex = startIndex, 0)
          for (let index = startIndex; index <= endIndex; index++) {
            instance.activeTanks[index].tanklistimg = TankSelectedImage;

            if (instance.selectedTanks.indexOf(instance.selectedTanks.find(s => s.pointId === instance.activeTanks[index].pointId)) === -1)
              instance.selectedTanks.push(instance.activeTanks[index]);
          }
          instance.selectedTank = instance.activeTanks.find(s => s.pointId === pointId);
        }
        else //no tank was selected. select everything from the beginning to this one. 
        {
          event.target.src = TankSelectedImage;
          instance.selectedTank = instance.activeTanks.find(s => s.pointId === pointId);
          var endIndex = instance.activeTanks.indexOf(instance.selectedTank);
          for (let index = 0; index <= endIndex; index++) {
            instance.activeTanks[index].tanklistimg = TankSelectedImage;
            instance.selectedTanks.push(instance.activeTanks[index]);
          }
        }

      }
      else { //regular clicked an unselected tank. Clear selectedTanks and revert images, and then select clicked tank

        instance.selectedTanks = [];
        instance.activeTanks.forEach(function (tank) {
          tank.tanklistimg = TankActiveImage;
        });

        event.target.src = TankSelectedImage;
        instance.selectedTank = instance.activeTanks.find(s => s.pointId === pointId);
        instance.selectedTanks.push(instance.selectedTank);
        instance.selectedTank.tanklistimg = TankSelectedImage;


      }
    }
    else {
      if (event.ctrlKey) { //ctrl clicked a selected tank. Deselect it. 
        event.target.src = TankActiveImage;
        var deselectedTank = instance.activeTanks.find(s => s.pointId === pointId);
        deselectedTank.tanklistimg = TankActiveImage;

        let filteredTanks = instance.selectedTanks.filter(function (obj) {
          return obj.pointId !== pointId;
        });
        instance.selectedTanks = filteredTanks;
        if (instance.selectedTanks.length <= 1) //if we are reverting to single edit mode, set the last tank in the array to the selectedTank
          instance.selectedTank = instance.selectedTanks[0];
      }
      else //regular clicked a selected tank
      {

        instance.selectedTanks.forEach(function (tank) {
          tank.tanklistimg = TankActiveImage;
        });
        if (instance.selectedTanks.length > 1) { //if we are in batch edit mode and regular click a tank, single select that tank
          instance.activeTanks.forEach(function (tank) {
            tank.tanklistimg = TankActiveImage;
          });
          instance.selectedTank = instance.activeTanks.find(s => s.pointId === pointId);
          instance.selectedTanks = [];
          instance.selectedTanks.push(instance.selectedTank);
          event.target.src = TankSelectedImage;
          instance.selectedTank.tanklistimg = TankSelectedImage;
        }
        else //otherwise we clicked on the only one selected. deselect it. 
        {
          instance.selectedTanks = [];
          //event.target.src = TankActiveImage;
          instance.selectedTank.tanklistimg = TankActiveImage;
          instance.selectedTank = null;
        }
      }
    }
    instance.populateDeviceDropdowns(instance);
    instance.getActiveInactiveTanks();
    instance.getFilteredActiveTanks();
    instance.getTankViewTabs();
  }

  dragSelect(event:any){
    let instance = this;
    if(instance.activationActive === true)
    {
      return;
    }

    if (event.length > 0){
    instance.dragSelectActive = true;
    instance.selectedTanks = [];
    instance.activeTanks.forEach(function (tank) {
      tank.tanklistimg = TankActiveImage;
    });
    instance.selectedTank = event[0];
    event.forEach(function(tank){
      instance.selectedTanks.push(tank);
      tank.tanklistimg = TankSelectedImage;
    });
    instance.populateDeviceDropdowns(instance);
    instance.getActiveInactiveTanks();
    instance.getFilteredActiveTanks();
    instance.getTankViewTabs();
    setTimeout(function(){     instance.dragSelectActive = false; }, 100);
  }
  }

  onTankPanelClick(event: any) {
    let instance = this;
    if(instance.activationActive === true)
      return;
    if (instance.dragSelectActive == false)
    {
    instance.activeTanks.forEach(function (tank) {
      tank.tanklistimg = TankActiveImage;
    });
    instance.selectedTanks = [];
    //event.target.src = TankActiveImage;
    if (instance.selectedTank) {
      instance.selectedTank.tanklistimg = TankActiveImage;
      instance.selectedTank = null;
    }

    instance.getActiveInactiveTanks();
    instance.getFilteredActiveTanks();
    instance.getTankViewTabs();
  }
  else
  {instance.dragSelectActive = false;}
  }


  populateDeviceDropdowns(instance: any, fromParameterChange: boolean = false) {
    if(!instance.selectedTank){
      return;
    }

    //Populate device types from protocol
    var moduleNum = instance.selectedTank.point.pointConfiguration[Object.keys(instance.selectedTank.point.pointConfiguration).find(s => instance.selectedTank.point.pointConfiguration[s].parameter === 'Module')].pendingValue;
    var channelNum = instance.selectedTank.point.pointConfiguration[Object.keys(instance.selectedTank.point.pointConfiguration).find(s => instance.selectedTank.point.pointConfiguration[s].parameter === 'Channel')].pendingValue;
    var protocolName = allProtocols[this._rtuConfiguration.getProtocolforModuleAndChannel(moduleNum, channelNum)];

    var protocol = instance._rtuConfiguration.availableConfiguration.protocols.filter(protocol => {
      return protocol.name == protocolName
    })[0];


    var deviceTypesString = '';
    var deviceTypesValue = '';
    var setprotocolOptions = true;

    if (this.selectedTanks.length > 1)
    {
      // if we have multiple tanks selected we need to check a bunch of stuff before we can populate the device type.
      // first they must all be on the same protocol type
      let tanks = this.selectedTanks;
      var moduleNumLocal = "";
      var channelNumLocal = "";
      var protocolNameLocal = "";
      var protocolNameLast = "";

      // cannot use a function here because we cannot pass in what we want. That is why we use a loop
      for (var loop = 0;loop < this.selectedTanks.length;loop++)
      {
        var tank = this.selectedTanks[loop];
        moduleNumLocal = tank.point.pointConfiguration[Object.keys(tank.point.pointConfiguration).find(s => tank.point.pointConfiguration[s].parameter === 'Module')].pendingValue;
        channelNumLocal = tank.point.pointConfiguration[Object.keys(tank.point.pointConfiguration).find(s => tank.point.pointConfiguration[s].parameter === 'Channel')].pendingValue;
        protocolNameLocal = allProtocols[this._rtuConfiguration.getProtocolforModuleAndChannel(moduleNumLocal, channelNumLocal)];
        if(protocolNameLast == "")
        {
          protocolNameLast = protocolNameLocal;
        }
        else
        {
          if(protocolNameLast != protocolNameLocal)
          {
            setprotocolOptions = false;
            break;
          }
        }
      }
    }

    if(setprotocolOptions === true)
    {
      if (protocol != null) 
      {
        for (var deviceType in protocol.availableDeviceTypes) 
        {
          deviceTypesString += protocol.availableDeviceTypes[deviceType].id + ',';
          deviceTypesValue += protocol.availableDeviceTypes[deviceType].deviceTypeValue + ',';
        }
        deviceTypesString = (deviceTypesString.length > 0) ? deviceTypesString.substring(0, deviceTypesString.length - 1) : "NONE";
      }
    }
    else
    {
      deviceTypesString = "NONE";
      deviceTypesValue = "0";
    }

      //set the available device types 
      
      if (this.selectedTanks.length > 1) 
      {
        let tanks = this.selectedTanks;
        tanks.forEach(function (tank, index) {
          let matchingKey = Object.keys(tank.point.pointConfiguration).find(key => tank.point.pointConfiguration[key].parameter === "DeviceType");
          if (matchingKey) {
            tank.point.pointConfiguration[matchingKey].availableCommands = deviceTypesString;
            tank.point.pointConfiguration[matchingKey].availableDeviceTypeValues = deviceTypesValue;
          }
        });
      }

      var deviceTypeParameter = instance.selectedTank.point.pointConfiguration[Object.keys(instance.selectedTank.point.pointConfiguration).find(s => instance.selectedTank.point.pointConfiguration[s].parameter === 'DeviceType')];
      if (deviceTypeParameter.availableCommands != deviceTypesString) 
      {
        //the available devices have changed. Clear out the pending value. 
        deviceTypeParameter.availableCommands = deviceTypesString;
        deviceTypeParameter.availableDeviceTypeValues = deviceTypesValue;
      }

      if(deviceTypesString.toUpperCase() === "NONE")
        deviceTypeParameter.availableDeviceTypeValues = deviceTypesValue;

      // check if the combo box should be disabled
      if(deviceTypeParameter.parameter === 'DeviceType' && (deviceTypesString.toUpperCase() === "NONE" || deviceTypesString === ""))
        instance.disableDeviceTypeDropDown = true;
      else if(deviceTypeParameter.parameter === 'DeviceType')
        instance.disableDeviceTypeDropDown = false;


      let tanks = this.selectedTanks;
      tanks.forEach(function (tank, index) 
      {
        var perTankdeviceTypeParameter = instance.selectedTank.point.pointConfiguration[Object.keys(instance.selectedTank.point.pointConfiguration).find(s => instance.selectedTank.point.pointConfiguration[s].parameter === 'DeviceType')];
        //populate device commands from device type
        if (perTankdeviceTypeParameter.pendingValue != "0" && perTankdeviceTypeParameter.pendingValue != "" )
        {
          // need to remap to actual id values.
          var selectedDeviceNum = parseInt(perTankdeviceTypeParameter.pendingValue) - 1;

          var deviceCommands = '';
          if(perTankdeviceTypeParameter.parameter === 'DeviceType')
          {
            // if none is the only selection disable the drop down
            const availableDeviceTypeValues = deviceTypesValue.split(',');

            var bDeviceTypeFound = false;
            // reset the selecteddevicenumber since device commands are not sequential            
            for (var loop = 0;loop < availableDeviceTypeValues.length;loop++)
            {
              if(availableDeviceTypeValues[loop] === perTankdeviceTypeParameter.pendingValue)
              {
                selectedDeviceNum = loop;
                bDeviceTypeFound = true;
                break;
              }
            }
            //only need to update the pending value for all tanks in the array if we are coming from a parameter change
            // (if we are coming from a re-render such as a multiselect, don't update them yet)
            if(bDeviceTypeFound === false && (fromParameterChange || tanks.length === 1))
            {
              // if we did not find it the user could of changed the port protocol so just set at undefined
              let originalPendingValue = perTankdeviceTypeParameter.pendingValue;
              perTankdeviceTypeParameter.pendingValue = selectedDeviceNum = 0;
              instance.checkForIncrementGlobalPendingChanges(perTankdeviceTypeParameter, originalPendingValue);
            }
          }
          //only populate device commands in single select mode. 
          // make sure that we do not exceed the available selections
          if (tanks.length == 1)// && perTankdeviceTypeParameter.parameter === 'DeviceType')
          {
            if(protocol)
            {
              // the list of device commands depend on the device type.
              // The XML will send the whole list in the definition of the parameter but the supported by the
              // device are being sent in the devices section on the XML. We want to provide the full list and the subset list to the inline editor
              for (var deviceCommand in protocol.availableDeviceTypes[selectedDeviceNum].availableCommands) 
              {
                deviceCommands += protocol.availableDeviceTypes[selectedDeviceNum].availableCommands[deviceCommand] + ',';
              }
              deviceCommands = (deviceCommands.length > 0) ? deviceCommands.substring(0, deviceCommands.length - 1) : "";
            }
      
            // disable the device command if these are nms gauges the gauge numbers must alway be unique bds
            if(perTankdeviceTypeParameter.pendingValue === "11" ||
            perTankdeviceTypeParameter.pendingValue === "12")
            {
              instance.disableDeviceCommandDropDown = true;
              instance.disableNmsDeviceCommandDropDown = false;
              // set the available device commands for the NMSDeviceCommand in a property of the object 'availableDeviceTypeValues'
              var nmsdevicevalues = tank.point.pointConfiguration[Object.keys(tank.point.pointConfiguration).find(s => tank.point.pointConfiguration[s].parameter === 'NMSDeviceCmd')];
              if (nmsdevicevalues! = null && nmsdevicevalues.hasOwnProperty('availableDeviceTypeValues')) {
                nmsdevicevalues.availableDeviceTypeValues = deviceCommands;
              }
            }
            else if(deviceCommands.toUpperCase() === "NONE" || deviceCommands === "")
            {
              instance.disableDeviceCommandDropDown = true;
              instance.disableNmsDeviceCommandDropDown = true;
            }
            else
            {
              instance.disableDeviceCommandDropDown = false;
              instance.disableNmsDeviceCommandDropDown = true;
            }

            if (instance.disableNmsDeviceCommandDropDown === false) {
              deviceCommands = "";
            } else {
              var nmsdevicevalues = tank.point.pointConfiguration[Object.keys(tank.point.pointConfiguration).find(s => tank.point.pointConfiguration[s].parameter === 'NMSDeviceCmd')];
              if (nmsdevicevalues! = null && nmsdevicevalues.hasOwnProperty('availableDeviceTypeValues')) {
                nmsdevicevalues.availableDeviceTypeValues = "";
              }            }
            // set the available device commands in a property of the object 'availableDeviceTypeValues'
            tank.point.pointConfiguration[Object.keys(tank.point.pointConfiguration).find(s => tank.point.pointConfiguration[s].parameter === 'DeviceCmd')].availableDeviceTypeValues = deviceCommands;
          }
        }
        else
        {
          // no changes so we just need to check the values and set the enabled disabled flags
          if(perTankdeviceTypeParameter.pendingValue === "0" ||
            perTankdeviceTypeParameter.availableCommands.toUpperCase() === "NONE" ||
            perTankdeviceTypeParameter.availableCommands === "" ||
            perTankdeviceTypeParameter.pendingValue === "11" ||
            perTankdeviceTypeParameter.pendingValue === "12")
            {
            instance.disableDeviceCommandDropDown = true;
            instance.disableNmsDeviceCommandDropDown = false;
            }
          else
          {
            instance.disableDeviceCommandDropDown = false;
            instance.disableNmsDeviceCommandDropDown = true;
          }

      }
    }); //end function
  }

    
  onactivationPanelClick(event:any)
  {
    event.stopPropagation();
    let instance = this;
    if(instance.dragSelectActive === false){
      return;
    }

// This is presenting a problem, as click on the panel is spurious
/*
      // if we get this just remove all selected items
      let instance = this;
      instance.tanksToChangeVisibility.forEach(function (localtank,index) 
      {
        let activeTank;
        var tt = 0;
        ++tt;
        var indexid = instance.tanksToChangeVisibility.map(s => s.id).indexOf(localtank.id);
        if (indexid != -1) 
        {
          if (instance.tankActivationMode === 'ACTIVATE TANKS') 
          {
            activeTank = instance.inactiveTanks.find(s => s.pointId === localtank.id);
            if(activeTank != null)
              activeTank.activationimg = TankInactiveImage;
          }
          else
          {
            activeTank = instance.activeTanks.find(s => s.pointId === localtank.id);
            if(activeTank != null)
              activeTank.activationimg = TankActiveImage;
          }
        }
      });
      
      instance.tanksToChangeVisibility = [];
      instance.dragSelectActive = false;
    }

    instance.dragSelectActive = false;
*/    
  }

  activationtanklistdragSelect(event:any)
  {
    if (event.length > 0)
    {
      let instance = this;

      instance.dragSelectActive = false;

      // first we need to clear the list
      instance.tanksToChangeVisibility.forEach(function (localtank,index) 
      {
        let activeTank;
        var tt = 0;
        ++tt;
        var indexid = instance.tanksToChangeVisibility.map(s => s.id).indexOf(localtank.id);
        if (indexid != -1) 
        {
          if (instance.tankActivationMode === 'ACTIVATE TANKS') 
          {
            activeTank = instance.inactiveTanks.find(s => s.pointId === localtank.id);
            if(activeTank != null)
              activeTank.activationimg = TankInactiveImage;
          }
          else
          {
            activeTank = instance.activeTanks.find(s => s.pointId === localtank.id);
            if(activeTank != null)
              activeTank.activationimg = TankActiveImage;
          }
        }
      });
      instance.tanksToChangeVisibility = [];

      this.getActiveInactiveTanks();
      this.getFilteredActiveTanks();
      this.getFilteredActivationTanks();
      this.getTankViewTabs();

      if (instance.tankActivationMode === 'ACTIVATE TANKS') 
      {
          // add the tanks as being active
        var index = 0;
        for (index = 0; index < event.length; index++) 
        {
          let value = '2';
          let localtank = event[index];
          var selectedTank = instance.inactiveTanks.find(s => s.pointId === localtank.pointId);
          if(selectedTank != null && selectedTank.activationimg !== TankActiveImage)
          {
            selectedTank.activationimg = TankActiveImage;
            let localpointData = { id: selectedTank.pointId, identifier: instance.tankVisibleIdentifier + selectedTank.tankId, value: value };
            instance.tanksToChangeVisibility.push(localpointData);
          }
        }
      }
      else
      {
        // add the tanks as being deactivated 
        var index = 0;
        for (index = 0; index < event.length; index++) 
        {
          let value = '1';
          let localtank = event[index];
          selectedTank = instance.activeTanks.find(s => s.pointId === localtank.pointId);
          if(selectedTank != null && selectedTank.activationimg !== TankInactiveImage)
          {
            selectedTank.activationimg = TankInactiveImage;
            let localpointData = { id: selectedTank.pointId, identifier: instance.tankVisibleIdentifier + selectedTank.tankId, value: value };
            instance.tanksToChangeVisibility.push(localpointData);
          }
          instance.dragSelectActive = true;
        }
      }

      this.getActiveInactiveTanks();
      this.getFilteredActiveTanks();
      this.getFilteredActivationTanks();
      this.getTankViewTabs();
      setTimeout(function(){     instance.dragSelectActive = false; }, 100);
    }
  }


  onActivationTankImgClick(event: any, pointId: number, tankId: number) {
    let instance = this;
    let value;
    let tank;
    let changeSelectedTank = false;

    instance.dragSelectActive = false;

    event.stopPropagation();

    if (this.tankActivationMode === 'ACTIVATE TANKS') 
    {
      value = '2';
      tank = this.filteredActivationTanks.find(s => s.pointId === pointId);
    }
    else 
    {
      value = '1';
      tank = this.activeTanks.find(s => s.pointId === pointId);
    }

    let pointData = { id: pointId, identifier: this.tankVisibleIdentifier + tankId, value: value };

    if (event.ctrlKey) 
    {
      changeSelectedTank = true;
    }
    else if (event.shiftKey) 
    {
      if(instance.tanksToChangeVisibility.length > 0)
      {
          let currentselectedimage = tank.activationimg;
          let selectedTank;
          // the start index will be the last tank in the above array
          var starttank = instance.tanksToChangeVisibility[instance.tanksToChangeVisibility.length - 1];
          // tank contains the last one in the selection
          var processSelection = 0;
          // the list has been sorted based on the name so we need to use it as our base.
          if (instance.tankActivationMode === 'ACTIVATE TANKS') 
          {
            var index = 0;
            for (index = 0; index < this.filteredActivationTanks.length; index++) 
            {
              selectedTank = this.filteredActivationTanks[index];
              if(processSelection === 0 &&
                (starttank.id === selectedTank.tankId ||
                tank.tankId === selectedTank.tankId))
              {
                processSelection = 1;
              }
              else if(processSelection === 1 &&
                (starttank.id === selectedTank.tankId ||
                tank.tankId === selectedTank.tankId))
              {
                processSelection = 2;
              }

              if(processSelection > 0)
              {
                if(selectedTank != null && selectedTank.activationimg !== TankActiveImage)
                {
                  selectedTank.activationimg = TankActiveImage;
                  let localpointData = { id: selectedTank.pointId, identifier: this.tankVisibleIdentifier + selectedTank.tankId, value: value };
                  this.tanksToChangeVisibility.push(localpointData);
                }
                if(processSelection > 1)
                  processSelection = 0;
              }
            }
          }
          else
          {
            var index = 0;
            for (index = 0; index < this.activeTanks.length; index++) 
            {
              selectedTank = this.activeTanks[index];
              if(processSelection === 0 &&
                (starttank.id === selectedTank.tankId ||
                tank.tankId === selectedTank.tankId))
              {
                processSelection = 1;
              }
              else if(processSelection === 1 &&
                (starttank.id === selectedTank.tankId ||
                tank.tankId === selectedTank.tankId))
              {
                processSelection = 2;
              }

              if(processSelection > 0)
              {
                if(selectedTank != null && selectedTank.activationimg !== TankInactiveImage)
                {
                  selectedTank.activationimg = TankInactiveImage;
                  let localpointData = { id: selectedTank.pointId, identifier: this.tankVisibleIdentifier + selectedTank.tankId, value: value };
                  this.tanksToChangeVisibility.push(localpointData);
                }
                if(processSelection > 1)
                  processSelection = 0;
              }
            }
          }
        }
    }
    else 
    { 
      changeSelectedTank = true;

      // check if this is a toggle if there is only one tank and it is the selected tank just toggle the image
      if(instance.tanksToChangeVisibility.length === 1)
      {
        if(tank.tankId === instance.tanksToChangeVisibility[0].id)
        {
          if(tank.activationimg === TankActiveImage && instance.tankActivationMode === 'ACTIVATE TANKS')
          {
            tank.activationimg = TankInactiveImage;
            changeSelectedTank = false;
          }
          else if(tank.activationimg === TankInactiveImage && instance.tankActivationMode === 'DEACTIVATE TANKS')
          {
            tank.activationimg = TankActiveImage;
            changeSelectedTank = false;
          }
        }
      }
      
      instance.tanksToChangeVisibility.forEach(function (localtank,index) 
      {
        let activeTank;
        var tt = 0;
        ++tt;
        var indexid = instance.tanksToChangeVisibility.map(s => s.id).indexOf(localtank.id);
        if (indexid != -1) 
        {
          if (instance.tankActivationMode === 'ACTIVATE TANKS') 
          {
            activeTank = instance.inactiveTanks.find(s => s.pointId === localtank.id);
            if(activeTank != undefined)
            {
              activeTank.activationimg = TankInactiveImage;
             }
          }
          else
          {
            activeTank = instance.activeTanks.find(s => s.pointId === localtank.id);
            if(activeTank != undefined)
            {
              activeTank.activationimg = TankActiveImage;
            }
          }
        }
      });
      instance.tanksToChangeVisibility = [];
      
    }


    if(changeSelectedTank === true)
    {
       if (tank.activationimg === TankActiveImage)  // user selecetd an tank
      {
        event.target.src = TankInactiveImage;
        tank.activationimg = TankInactiveImage;
        if (this.tankActivationMode === 'DEACTIVATE TANKS') 
        {
          this.tanksToChangeVisibility.push(pointData);
        }
        else 
        {
          var index = this.tanksToChangeVisibility.map(s => s.id).indexOf(pointId);
          if (index != -1) 
          {
            this.tanksToChangeVisibility.splice(index, 1);
          }
        }
      }
      else 
      {
        event.target.src = TankActiveImage;
        tank.activationimg = TankActiveImage;
        if (this.tankActivationMode === 'ACTIVATE TANKS') 
        {
          this.tanksToChangeVisibility.push(pointData);
        }
        else 
        {
          var index = this.tanksToChangeVisibility.map(s => s.id).indexOf(pointId);
          if (index != -1) 
          {
            this.tanksToChangeVisibility.splice(index, 1);
          }
        }
      }
    }
  }

  toggleTankSearch() {
    this.searchTankToggle = !this.searchTankToggle;
    this.searchTankString = '';
    this.getFilteredActiveTanks();
    this.getTankViewTabs();
    if (this.searchTankToggle) {
      const input = document.getElementById('tanksfilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  searchTankChanged(newSearchValue) {
    this.searchTankString = newSearchValue;
    console.log('search Tank Value: ' + this.searchTankString);
    this.getFilteredActiveTanks();
    this.getTankViewTabs();
  }

  toggleConfigurationSearch() {
    this.searchConfigurationToggle = !this.searchConfigurationToggle;
    this.searchConfigurationString = '';
    this.getTankViewTabs();
    if (this.searchConfigurationToggle) {
      const input = document.getElementById('configurationfilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  searchConfigurationChanged(newSearchValue) {
    this.searchConfigurationString = newSearchValue;
    console.log('search Configuration Value: ' + this.searchConfigurationString);
    this.getTankViewTabs();
  }

  toggleActivationSearch() {
    this.searchActivationToggle = !this.searchActivationToggle;
    this.searchActivationString = '';
    this.getFilteredActivationTanks();
    if (this.searchActivationToggle) {
      const input = document.getElementById('activationfilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  searchActivationChanged(newSearchValue) {
    this.searchActivationString = newSearchValue;
    console.log('search Activation Value: ' + this.searchActivationString);
    this.getFilteredActivationTanks();
  }


  onActivate() {
    this.activationActive = true;
    this.tanksToChangeVisibility = [];
    this.tankActivationMode = 'ACTIVATE TANKS';
    this.inactiveTanks.forEach(function (tank) {
      tank.activationimg = TankInactiveImage;
    });
    this.tankActivationApplyButtonText = 'Activate';
    this.getFilteredActivationTanks();
  }

  onDeactivate() {
    this.activationActive = true;
    this.tanksToChangeVisibility = [];
    this.tankActivationMode = 'DEACTIVATE TANKS';
    this.activeTanks.forEach(function (tank) {
      tank.activationimg = TankActiveImage;
    });
    this.tankActivationApplyButtonText = 'Deactivate';
    this.getFilteredActivationTanks();
  }

  saveToRTUConfiguration() {
    const instance = this;

    // if we are activating tanks we need to prompt for the new label
    if (this.tankActivationMode === 'ACTIVATE TANKS') {
      this.setActivateMultiLabel = '';
      this.activateLabelModalBackground.nativeElement.classList.remove('d-none');
      this.batchRenameLabel.nativeElement.focus();
    } else {
      const newLabels: IPointData[] = [];

      for (let index = 0; index < this.tanksToChangeVisibility.length; index++) {
        const test = instance.tanksToChangeVisibility[index];

        const tank = instance.rtuconfiguration.points[instance.tanksToChangeVisibility[index].id];
        const label = "Tank " + ("000" + (instance.tanksToChangeVisibility[index].id + 1)).slice(-3);
        const labelIdentifier = Object.keys(tank.pointConfiguration).find(s => tank.pointConfiguration[s].parameter === 'Label');
        if (labelIdentifier) {
          newLabels.push({ id: instance.tanksToChangeVisibility[index].id, identifier: parseInt(labelIdentifier), value: label });
        }
      }

      this._rtuConfiguration.setPointData(newLabels);
      this._rtuConfiguration.setPointData(this.tanksToChangeVisibility);
      this.tanksToChangeVisibility = [];
    }
  }

  cancelBatchActivationRename() {
    this.activateLabelModalBackground.nativeElement.classList.add('d-none');
  }

  applyBatchActivationRename() {
    const newLabels: IPointData[] = [];
    const instance = this;
    let digits = 1;
    if(this.tanksToChangeVisibility.length > 9
    && this.tanksToChangeVisibility.length < 100){
      digits = 2;
    }
    else if(this.tanksToChangeVisibility.length > 99){
      digits = 3;
    }
    this.tanksToChangeVisibility.sort(function (a, b) { return a.id - b.id });


    for (let index = 0; index < this.tanksToChangeVisibility.length; index++) {
      const tank = instance.rtuconfiguration.points[instance.tanksToChangeVisibility[index].id];
      const label = instance.setActivateMultiLabel + (this.tanksToChangeVisibility.length === 1 ? '' : ('000' + (index + 1).toString()).slice(-digits));
      const labelIdentifier = Object.keys(tank.pointConfiguration).find(s => tank.pointConfiguration[s].parameter === 'Label');
      if (labelIdentifier) {
        newLabels.push({ id: instance.tanksToChangeVisibility[index].id, identifier: parseInt(labelIdentifier), value: label });
      }
    }
    this._rtuConfiguration.setPointData(newLabels);
    this._rtuConfiguration.setPointData(this.tanksToChangeVisibility);
    this.activateLabelModalBackground.nativeElement.classList.add('d-none');
    this.tanksToChangeVisibility = [];
    this.activationActive = false;
  }

  VerifyCancelActivationPrompt(): void {
    // check if there are pending changes and prompt if there are. If not just close.
    if (this.tanksToChangeVisibility.length > 0)
      this.modalRef = this._modalService.show(this.modalVerifyCancelActivation, this.modalConfig);
    else
      this.cancelActivation();
  }

  cancelActivation() {
    this.activationActive = false;
    if (this.modalRef !== undefined)
      this.modalRef.hide();
  }

  VerifyApplytoRTUPrompt(): void {
    this.modalRef = this._modalService.show(this.modalVerifyApplytoRTUAction, this.modalConfig);
  }

  applyToRTU() {
    this._rtuConfiguration.applyDataToRTU(false, this._rtuConfiguration.checkForRTUConfigChanges());
    if (this.modalRef !== undefined)
      this.modalRef.hide();
  }

  VerifyCancelChangesPrompt(): void {
    if (this._rtuConfiguration.checkForRTUConfigChanges())
      this.modalRef = this._modalService.show(this.modalVerifyCancelAction, this.modalConfig);
  }

  public areThereNoChangesMade() {
    if (this._rtuConfiguration.checkForRTUConfigChanges())
      return false;
    else
      return true;
  }

  cancelChanges() {
    this._rtuConfiguration.cancelPendingChanges();
    // redraw the screen 
    if(this.selectedTank != undefined){
      this.populateDeviceDropdowns(this);
    }
    this.getActiveInactiveTanks();
    this.getFilteredActiveTanks();
    this.getFilteredActivationTanks();
    this.getTankViewTabs();
    this.modalRef.hide();
  }

  public saveRtuConfigToDisk() {
    let configToSave;
    if (this.rtuconfiguration) {
      configToSave = JSON.stringify(this.rtuconfiguration);
    }
    else {
      configToSave = 'no data';
    }

    const data = new Blob([configToSave], { type: 'application/json' });

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(data, 'config.rtuconfig');
    }
    else {
      saveAs(data, 'config.rtuconfig');
    }
  }

  public getcancelButtontext() {
    if (this.tanksToChangeVisibility.length > 0)
      return "Cancel";
    else
      return "Close";
  }

  public areNoTanksSelected() {
    if (this.tanksToChangeVisibility.length > 0)
      return false;
    else
      return true;
  }

  public isRTUConnectedandChangesExist() {
    if (this._rtuConfiguration.connectionStatus !== RTUConnectionStatus.CONNECTED) {
      return true;
    }
    else if (this._rtuConfiguration.checkForRTUConfigChanges() === false) {
      return true;
    }
    else {
      return false;
    }
  }

  public setTextFieldStyle()
  {
    let styles = {};
  
    if(this.autoIncrementParameterName == 'DeviceID')
    {
      styles = { 'visibility':'hidden'};
    }
    else
    {
      styles = { 'visibility':'visible'};
    }
  
    return styles;
  }


  autoIncrementPrompt(event: any, parameterName: string): void {
    this.autoIncrementParameterName = parameterName;
    this.modalRef = this._modalService.show(this.modalAutoIncrementValue, this.modalConfig);
  }

  autoIncrementApply(baseText:any, startingInterger: any, incrementInterval: any) {
    const instance = this;
    let starting = parseInt(startingInterger);
    let increment = parseInt(incrementInterval);
    let parameterName = this.autoIncrementParameterName;


    let batchParameterKey = Object.keys(this.batchEditTank.point.pointConfiguration).find(key => this.batchEditTank.point.pointConfiguration[key].parameter === this.autoIncrementParameterName);
    let baseString = baseText;//this.batchEditTank.point.pointConfiguration[batchParameterKey].pendingValue;

    let tanks = this.selectedTanks;
    tanks.sort(this.comparePointId);

    tanks.forEach(function (tank, index) {
      let matchingKey = Object.keys(tank.point.pointConfiguration).find(key => tank.point.pointConfiguration[key].parameter === parameterName);
      if (matchingKey) {
        let originalPendingValue = tank.point.pointConfiguration[matchingKey].pendingValue;
        if (parameterName == "DeviceID")
          tank.point.pointConfiguration[matchingKey].pendingValue = String(starting);
        else
          tank.point.pointConfiguration[matchingKey].pendingValue = baseString + String(starting);
        //inline configurator will take care of this for the first change, but not the others since they are programmatic and not coming from the configurator.
        instance.checkForIncrementGlobalPendingChanges(tank.point.pointConfiguration[matchingKey], originalPendingValue);
      }
      starting += increment;
    });

    instance.getActiveInactiveTanks();
    instance.getFilteredActiveTanks();
    instance.getTankViewTabs();
    this.modalRef.hide();
  }

  public checkForIncrementGlobalPendingChanges(parameter: IParameter, originalPendingValue: string) {
    let instance = this;
    if (instance._rtuConfiguration.isParameterValueChanged(parameter)) {
      if (originalPendingValue == parameter.value)
        instance._rtuConfiguration.incrementGlobalPendingChanges();
    }
    else {
      if (originalPendingValue != parameter.value)
        instance._rtuConfiguration.decrementGlobalPendingChanges();
    }
  }

  comparePointId(point1: any, point2: any) {
    if (point1.pointId < point2.pointId) {
      return -1;
    }
    if (point1.pointId > point2.pointId) {
      return 1;
    }
    return 0;
  }

  public setIncrementTitlestyle()
  {
    let styles = {};
    if(this.autoIncrementParameterName == 'DeviceID')
    {
      styles = { 'visibility':'hidden'};
    }
    else
    {
      styles = { 'visibility':'visible'};
    }
  
    return styles;
  }
  public setIncrementTitlestyle1()
  {
    let styles = {};
  
    if(this.autoIncrementParameterName != 'DeviceID')
    {
      styles = { 'visibility':'hidden'};
    }
    else
    {
      styles = { 'visibility':'visible', 'position':'absolute', 'left':'20px'};
    }
  
    return styles;
  }

  public setLabelTextboxMaxLength(nameid:string)
  {

    let styles = {};
    let instance = this;
    // set the length based on the selected tanks
    if(instance.selectedTanks.length > 0)
    {
      var tank = instance.selectedTanks[0];
      var maxallowedlength = "32";

      let labelIdentifier = Object.keys(tank.point.pointConfiguration).find(s => tank.point.pointConfiguration[s].parameter === 'Label');
      if(labelIdentifier != undefined)
      {
        maxallowedlength = (tank.point.pointConfiguration[labelIdentifier].datatypeLength - 3).toString();
      }

      const textinputbox = document.getElementById("baseText");

      if(textinputbox != undefined)
      {
        textinputbox.setAttribute("maxLength",maxallowedlength);
      }
    }

    return styles;

  }

}
