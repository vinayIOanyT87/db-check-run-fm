import { Component, OnInit, TemplateRef, ViewChild, ModuleWithComponentFactories, ElementRef } from '@angular/core';
import { RtuconfigurationService, IRTUConfiguration, Diagview, RtuDataValue, IFilter } from 'src/app/services/rtuconfiguration.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { Type } from '@angular/compiler';
// import { element } from '@angular/core/src/render3';
import { IParameter } from 'src/app/services/availablemodules.service';
import * as saveAs from 'node_modules/file-saver';
import { DatatableComponent } from '@swimlane/ngx-datatable';
import { BsDatepickerModule,BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-diagnosticstagviewer',
  templateUrl: './diagnosticstagviewer.component.html',
  styleUrls: ['./diagnosticstagviewer.component.css']
})

export class DiagnosticstagviewerComponent implements OnInit {
  diagnosticViewSubscription : Subscription;
  currentView: Diagview;
  selected: string;
  tableSelected = [];
  checkedForDelete: [];
  pickerType: string = 'none';
  pickerModule: string = 'none';
  pickerChannel: string = 'none';
  rtuconfiguration: IRTUConfiguration;
  tagsToPick: IParameter[] = [];
  noResult = true;
  deleteEnabled = false;
  liveData: IParameter[];
  filterMode = false;
  addText: string = "+ Add";
  filterText: string = "";
  elligibleParameters: IParameter[];
  rtuconfigurationSubscription: Subscription;
  public dpConfig: Partial<BsDatepickerConfig> = new BsDatepickerConfig();
  
  @ViewChild('modalAddTag', { static: true }) public modalAddTag: TemplateRef<any>;
  @ViewChild('modalAddFilter', { static: true }) public modalAddFilter: TemplateRef<any>;
  @ViewChild(DatatableComponent, { static: true }) table: DatatableComponent;
  @ViewChild('filterEntryText', { static: true }) public filterField: ElementRef;


  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false
  };
  availableTypes = [
    { name: 'Module', value: 'module' },
    { name: 'Tank', value: 'tank' },
    { name: 'Alarm', value: 'alarm' }
  ]
  columns = [
    { name: 'Parameter', prop: 'readableName', width: 300, draggable: false },
    { name: 'Description', prop: 'description', width: 260, draggable: false },
    { name: 'Value', prop: 'value', width: 180, draggable: false },
    { name: 'Units', prop: 'dataType', width: 140, draggable: false },
    { name: 'Timestamp', prop: 'serverTimeStamp', width: 270, draggable: false },
    { name: 'Status', prop: 'readableStatus', width: 80, draggable: false },
  ];


  rows: IParameter[] = [];

  constructor(private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService) {
      this.dpConfig.containerClass = 'theme-default';
    }


  ngOnInit() {
    this.SubscribeActiveDiagnosticView();
    this.getRTUConfiguration();
    this.refreshLiveData();
  }

  ngOnDestroy(){
    this.rtuconfigurationSubscription.unsubscribe();
    this.diagnosticViewSubscription.unsubscribe();
  }

  getRTUConfiguration(): any {
    this.rtuconfigurationSubscription = this._rtuConfiguration.get().subscribe(data => {
      if (data.RTUConfiguration) {
        this.rtuconfiguration = data.RTUConfiguration;
        if (this.currentView.filterCollection.dataType != 0) {
          if (this.elligibleParameters != undefined && this.elligibleParameters.length > 0)
            this._rtuConfiguration.unsubscribeRealtimeParameters(this.elligibleParameters);
          this.elligibleParameters = this.populateElligibleParameters(this.currentView.filterCollection.dataType);
          this._rtuConfiguration.subscribeRealtimeParameters(this.elligibleParameters);
          this.evaluateFilter(this.elligibleParameters);
          if (this.filterMode)
          {
            this.updateFilter(this.filterText);
          }
        }
      } else {
        this.rtuconfiguration = null;
      }
    });
  }

  refreshLiveData(): any {
    this._rtuConfiguration.getLiveDataValues().subscribe(data => {
      var diagnosticRows = this.rows;
      if (this.currentView.filterCollection.dataType != 0) {
        //this.evaluateFilter(data)
      }
      else {
        this.liveData = data;

        this.liveData.forEach(function (parameter) {
          var rowToUpdate = diagnosticRows.find(row => { return parameter.identifier == row.identifier });
          if (rowToUpdate !== undefined) {
            rowToUpdate.value = parameter.value;
            rowToUpdate.serverTimeStamp = parameter.serverTimeStamp;
            rowToUpdate.readableStatus = parameter.readableStatus;
            rowToUpdate.status = parameter.status;
          }
        });
        this.rows = diagnosticRows;
        this.rows = [...this.rows];
      }

    });

  }

  evaluateFilter(parameters: IParameter[]) {
    const filterDataType = +this.currentView.filterCollection.dataType;
    let temp: IParameter[] = [];

    switch (filterDataType) {
      case 1: //numeric
        temp = parameters.filter(parameter => {
          let includeParameter = false;
          let breakEarly = false;
          this.currentView.filterCollection.filters.forEach(filter => {
            if (breakEarly)
              return false;
              switch (filter.comparator) {
                case "GREATERTHAN":
                  includeParameter = (parseFloat(parameter.value) > parseFloat(filter.value));
                  break;
                case "GREATERTHANOREQUALTO":
                  includeParameter = (parseFloat(parameter.value) >= parseFloat(filter.value));
                  break;
                case "EQUALTO":
                  includeParameter = (parseFloat(parameter.value) == parseFloat(filter.value));
                  break;
                case "LESSTHANOREQUALTO":
                  includeParameter = (parseFloat(parameter.value) <= parseFloat(filter.value));
                  break;
                case "LESSTHAN":
                  includeParameter = (parseFloat(parameter.value) < parseFloat(filter.value));
                  break;
                case "NOT":
                  includeParameter = (parseFloat(parameter.value) != parseFloat(filter.value));
                  break;
              }
              if (!includeParameter)
                breakEarly = true;          
          });
          return includeParameter;
        });
        break;
      case 2://timestamp
        temp = parameters.filter(parameter => {
          let includeParameter = false;
          let breakEarly = false;
          this.currentView.filterCollection.filters.forEach(           
            filter => {
              var filterVal = new Date(filter.date);
              if(typeof filter.date === 'string' )
                filter.date = filterVal;
              if (filter.time ==null)
                return false;
              if (breakEarly)
                return false;
              filterVal.setHours(filter.time.hour, filter.time.minute, filter.time.second) ;
              var parameterVal = new Date(parameter.value);
              parameterVal.setSeconds(0,0);
              filterVal.setSeconds(0,0);              
              switch (filter.comparator) {
                case "GREATERTHAN":
                  includeParameter = (parameterVal > filterVal);
                  break;
                case "GREATERTHANOREQUALTO":
                  includeParameter = (parameterVal >= filterVal);
                  break;
                case "EQUALTO":
                  includeParameter = (parameterVal.getTime() == filterVal.getTime());
                  break;
                case "LESSTHANOREQUALTO":
                  includeParameter = (parameterVal <= filterVal);
                  break;
                case "LESSTHAN":
                  includeParameter = (parameterVal <filterVal);
                break;
                case "NOT":
                  includeParameter = (parameterVal.getTime() != filterVal.getTime());
                  break;
              }
              if (!includeParameter)
                breakEarly = true;  
          });
          return includeParameter;
        });
        break;
      case 3://string
        temp = parameters.filter(parameter => {
          let includeParameter = false;
          let breakEarly = false;
          this.currentView.filterCollection.filters.forEach(filter => {
            if (breakEarly)
            return false;
              switch (filter.comparator) {
                case "IS":
                  includeParameter = (parameter.value == filter.value);
                  break;
                case "CONTAINS":
                if (parameter.value !=null)
                  {
                    includeParameter = (parameter.value.includes(filter.value));
                  }
                  break;
              }
              if (!includeParameter)
                breakEarly = true;  

          });
          return includeParameter;
        });
        break;
    }

    this.rows = temp;
    // Whenever the filter changes, always go back to the first page
    this.table.offset = 0;
  }

  //modal for adding a tag
  addTagButton() {
    if (this.currentView.id != 'No view selected') {
      if (this.currentView.filterCollection.dataType == 0) {

        this.pickerType = 'none';
        this.pickerChannel = 'none';
        this.selected = '';
        this.noResult = true;

        if (this.filterMode)
          this.toggleFilter();

        this.populateTypeahead();
        this.modalRef = this._modalService.show(this.modalAddTag, this.modalConfig);
      }
      else {
        if (this.filterMode)
          this.toggleFilter();
        this.modalRef = this._modalService.show(this.modalAddFilter, this.modalConfig);
      }
    }
  }


  SubscribeActiveDiagnosticView(): void {
    this.diagnosticViewSubscription = this._rtuConfiguration.retrieveDiagnosticView().subscribe(data => {
      if (data) {
        if (this.filterMode) { this.toggleFilter(); }
        this.currentView = data;
        this.rows = [];
        if (this.currentView.filterCollection.dataType == 0) {
          this.addText = "+ Add";
          this.currentView.parameters.forEach(element => {
            //var row = { pt: element, value: '10-0-0', unit: 'ftin16th', timestamp: '-7:00', status: 'Bad' };
            this.rows.push(element);
          });


        }
        else {
          this.forceFilterEvaluation();
          this.addText = "+ Filter";

        }

      } else {
        this.currentView = { id: 'Could not retrieve active view', parameters: [], filterCollection: { dataType: 0, filters: [] } };
      }
    });
  }

  populateElligibleParameters(dataType: number) {
    const rtuService = this._rtuConfiguration;
    const currentConfiguration = this.rtuconfiguration;
    let elligibleParameters: IParameter[] = [];
    let moduleStrings = ["module0", "module1", "module2", "module3", "module4", "module5", "module6"];
    let channelStrings = ["channel1", "channel2", "channel3", "channel4", "channel5", "channel6", "channel7", "channel8"];
    var dataType = +dataType;
    switch (dataType) {
      case 1:
        var allowedTypes = ["int", "unsigned long", "long", "real", "unsigned int", "double"]
        moduleStrings.forEach(function (moduleString) {
          if (currentConfiguration[moduleString].name != 'Empty') {
            Object.keys(currentConfiguration[moduleString].moduleConfiguration).forEach(function (key) {
              if (allowedTypes.indexOf(currentConfiguration[moduleString].moduleConfiguration[key].dataType) > -1 && currentConfiguration[moduleString].moduleConfiguration[key].displayFormat != "TIME") {
                currentConfiguration[moduleString].moduleConfiguration[key].readableName = moduleString + ".moduleConfiguration." + currentConfiguration[moduleString].moduleConfiguration[key].parameter;
                currentConfiguration[moduleString].moduleConfiguration[key].readableStatus = rtuService.getStatusCode(currentConfiguration[moduleString].moduleConfiguration[key].status);
                ;

                elligibleParameters.push(currentConfiguration[moduleString].moduleConfiguration[key]);
              }
            });
            channelStrings.forEach(function (channelString) {
              Object.keys(currentConfiguration[moduleString][channelString].channelConfiguration).forEach(function (key) {
                if (allowedTypes.indexOf(currentConfiguration[moduleString][channelString].channelConfiguration[key].dataType) > -1 && currentConfiguration[moduleString][channelString].channelConfiguration[key].displayFormat != "TIME") {
                  currentConfiguration[moduleString][channelString].channelConfiguration[key].readableName = moduleString + "." + channelString + "." + currentConfiguration[moduleString][channelString].channelConfiguration[key].parameter;
                  currentConfiguration[moduleString][channelString].channelConfiguration[key].readableStatus = rtuService.getStatusCode(currentConfiguration[moduleString][channelString].channelConfiguration[key].status);
                  elligibleParameters.push(currentConfiguration[moduleString][channelString].channelConfiguration[key]);
                }
              });
            });
          }
        });
        break;
      case 2: //timestamp
        moduleStrings.forEach(function (moduleString) {
          if (currentConfiguration[moduleString].name != 'Empty') {
            Object.keys(currentConfiguration[moduleString].moduleConfiguration).forEach(function (key) {
              if (currentConfiguration[moduleString].moduleConfiguration[key].dataType == "int" && currentConfiguration[moduleString].moduleConfiguration[key].displayFormat == "TIME") {
                currentConfiguration[moduleString].moduleConfiguration[key].readableName = moduleString + ".moduleConfiguration." + currentConfiguration[moduleString].moduleConfiguration[key].parameter;
                currentConfiguration[moduleString].moduleConfiguration[key].readableStatus = rtuService.getStatusCode(currentConfiguration[moduleString].moduleConfiguration[key].status);
                ;

                elligibleParameters.push(currentConfiguration[moduleString].moduleConfiguration[key]);
              }
            });
            channelStrings.forEach(function (channelString) {
              Object.keys(currentConfiguration[moduleString][channelString].channelConfiguration).forEach(function (key) {
                if (currentConfiguration[moduleString][channelString].channelConfiguration[key].dataType == "int" && currentConfiguration[moduleString][channelString].channelConfiguration[key].displayFormat == "TIME") {
                  currentConfiguration[moduleString][channelString].channelConfiguration[key].readableName = moduleString + "." + channelString + "." + currentConfiguration[moduleString][channelString].channelConfiguration[key].parameter;
                  currentConfiguration[moduleString][channelString].channelConfiguration[key].readableStatus = rtuService.getStatusCode(currentConfiguration[moduleString][channelString].channelConfiguration[key].status);
                  elligibleParameters.push(currentConfiguration[moduleString][channelString].channelConfiguration[key]);
                }
              });
            });
          }
        });
        break;
      case 3: //string
      moduleStrings.forEach(function (moduleString) {
        if (currentConfiguration[moduleString].name != 'Empty') {
          Object.keys(currentConfiguration[moduleString].moduleConfiguration).forEach(function (key) {
            if (currentConfiguration[moduleString].moduleConfiguration[key].dataType == "string") {
              currentConfiguration[moduleString].moduleConfiguration[key].readableName = moduleString + ".moduleConfiguration." + currentConfiguration[moduleString].moduleConfiguration[key].parameter;
              currentConfiguration[moduleString].moduleConfiguration[key].readableStatus = rtuService.getStatusCode(currentConfiguration[moduleString].moduleConfiguration[key].status);
              ;

              elligibleParameters.push(currentConfiguration[moduleString].moduleConfiguration[key]);
            }
          });
          channelStrings.forEach(function (channelString) {
            Object.keys(currentConfiguration[moduleString][channelString].channelConfiguration).forEach(function (key) {
              if (currentConfiguration[moduleString][channelString].channelConfiguration[key].dataType == "string") {
                currentConfiguration[moduleString][channelString].channelConfiguration[key].readableName = moduleString + "." + channelString + "." + currentConfiguration[moduleString][channelString].channelConfiguration[key].parameter;
                currentConfiguration[moduleString][channelString].channelConfiguration[key].readableStatus = rtuService.getStatusCode(currentConfiguration[moduleString][channelString].channelConfiguration[key].status);
                elligibleParameters.push(currentConfiguration[moduleString][channelString].channelConfiguration[key]);
              }
            });
          });
        }
      });
        break
    }
    return elligibleParameters;
  }

  typeaheadNoResults(event: boolean) {
    this.noResult = event;
  }
  typeaheadOnBlur(event: any) {
    this.selected = event.item['parameter'];
  }

  populateTypeahead() {
    if (this.pickerType == 'module' && this.pickerModule != 'none' && this.pickerChannel != 'none') {
      var alreadyInUseParameters = this.rows;
      var options: IParameter[] = new Array();
      var module = this.rtuconfiguration[this.pickerModule];
      if (this.pickerChannel == 'moduleConfiguration') {
        Object.keys(module['moduleConfiguration']).forEach(function (key) {
          //options.push({'name': parameter.parameter, 'identifier': parameter.identifier});
          var foundEntry = alreadyInUseParameters.find(x => x.identifier === module['moduleConfiguration'][key].identifier);
          if (foundEntry == undefined)
            options.push(module['moduleConfiguration'][key]);
        });

      }
      else {
        var channel = module[this.pickerChannel];
        //  this.rtuconfiguration.module0.channel1.channelConfiguration.forEach(function(parameter)
        //   {
        //     this.tagsToPick.push(parameter.parameter)
        //   });

        var moduleString = this.pickerModule;
        var channelString = this.pickerChannel;

        Object.keys(channel['channelConfiguration']).forEach(function (key) {
          var readableName = moduleString + '.' + channelString + '.' + channel['channelConfiguration'][key].parameter;
          //options.push([{'name': parameter.parameter, 'identifier': parameter.identifier}]);
          var foundEntry = alreadyInUseParameters.find(x => x.readableName === readableName);
          if (foundEntry == undefined)
            options.push(channel['channelConfiguration'][key]);
        });
      }
      this.tagsToPick = options;
    }
    else {
      this.tagsToPick = [];
    }
  }

  add(type: string, module: string, channel: string, tag: string) {
    // unfortunately the ngx-bootstrap typehead only returns the named string and not the object, so we have to search the options to get our identifier
    // https://github.com/valor-software/ngx-bootstrap/issues/749
    var entry = this.tagsToPick.find(x => x.parameter === tag);
    //var identifier = entry;
    //for the table entry
    var readableName = module + '.' + channel + '.' + tag;
    entry.readableName = readableName;
    entry.readableStatus = this._rtuConfiguration.getStatusCode(entry.status);
    this.currentView.parameters.push(entry);
    this.rows.push(entry);
    this.rows = [...this.rows];
    this.modalRef.hide();
    this._rtuConfiguration.subscribeRealtimeParameters([entry]);

  }

  addCriteria() {
    if (this.currentView.filterCollection.dataType == 1)
    this.currentView.filterCollection.filters.push({ operatorType: "OR", comparator: "GREATERTHAN", value: "0", date: new Date, time: {hour: 0, minute: 0, second: 0} });
    else if (this.currentView.filterCollection.dataType == 2)
    this.currentView.filterCollection.filters.push({ operatorType: "OR", comparator: "GREATERTHAN", value: "0", date: new Date, time: {hour: 0, minute: 0, second: 0} });
    else if (this.currentView.filterCollection.dataType == 3)
    this.currentView.filterCollection.filters.push({ operatorType: "OR", comparator: "CONTAINS", value: "", date: new Date, time:{hour: 0, minute: 0, second: 0} });

  }
  deleteCriteria(filter: IFilter) {
    this.currentView.filterCollection.filters.splice(this.currentView.filterCollection.filters.indexOf(filter), 1);
  }

  dynamicFilterModalClose() {
    this.forceFilterEvaluation();
    this.modalRef.hide();
  }
  forceFilterEvaluation() {
    if (this.rtuconfiguration != undefined) {
      if (this.elligibleParameters != undefined && this.elligibleParameters.length > 0)
        this._rtuConfiguration.unsubscribeRealtimeParameters(this.elligibleParameters);

      this.elligibleParameters = this.populateElligibleParameters(this.currentView.filterCollection.dataType);
      this._rtuConfiguration.subscribeRealtimeParameters(this.elligibleParameters);
      this.evaluateFilter(this.elligibleParameters);
    }
  }

  onSelect({ selected }) {
    this.tableSelected.splice(0, this.tableSelected.length);
    this.tableSelected.push(...selected);
    this.tableSelected = this.tableSelected.filter(value => -1 !== this.rows.indexOf(value));
  }
  deleteChecked() {
    if (this.tableSelected.length > 0) {
      this.tableSelected.forEach(element => {
        this.rows = this.rows.filter(function (obj) {
          return obj !== element;
        }
        );
        this.currentView.parameters = this.currentView.parameters.filter(function (obj) {
          return obj !== element;
        }
        );
      });
      this.rows = [...this.rows];
      this.tableSelected = [];
    }
  }

  toggleFilter() {
    if (this.currentView.id != 'No view selected') {
      this.filterMode = !this.filterMode;
      if (!this.filterMode) {
        this.clearFilterText();
      }
      else{

        setTimeout(()=>{ // this will make the execution after the field becomes visible
          this.filterField.nativeElement.focus();
        },0);  

      }
    }
  }

  updateFilter(value) {
    const val = value;

    // filter our data
    if (this.currentView.filterCollection.dataType == 0)
    {
    const temp = this.currentView.parameters.filter(function (d) {
      return d.readableName.toLowerCase().indexOf(val) !== -1 || !val;
    });

    // update the rows
    this.rows = temp;
    // Whenever the filter changes, always go back to the first page
    this.table.offset = 0;
  }
  else
  {
    this.forceFilterEvaluation();
    const temp = this.rows.filter(function (d) {
      return d.readableName.toLowerCase().indexOf(val) !== -1 || !val;
    });

    // update the rows
    this.rows = temp;
    // Whenever the filter changes, always go back to the first page
    this.table.offset = 0;
  }
  }

  public saveRtuConfigToDisk() {
    let configToSave;
    if (this.rtuconfiguration) {
      configToSave = JSON.stringify(this.rtuconfiguration);
    } else {
      configToSave = 'no data';
    }

    const data = new Blob([configToSave], { type: 'application/json' });

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(data, 'config.rtuconfig');
    } else {
      saveAs(data, 'config.rtuconfig');
    }
  }


  clearFilterText() {
    
    this.filterText = '';
    if (this.currentView.filterCollection.dataType == 0)
    this.rows = JSON.parse(JSON.stringify(this.currentView.parameters));
    else
    this.forceFilterEvaluation();
  }
}
