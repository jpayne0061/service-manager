import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Service } from '../models/service';
import { MachineToServices } from '../models/machineToServices';
import { KillResponse } from '../models/killResponse';
import { SettingsBlob } from '../models/settingsBlob';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css']
})
export class FetchDataComponent {
  public machineNames: string = "";
  public machineToServices: MachineToServices[] = [];
  public servicesSet: boolean = false;
  public fetchingServices: boolean = false;
  public lastServiceLoaded: boolean = false;
  public fetchedMachines: number = 0;
  public serviceNameFilter: string = "";
  public advancedOptionsActive: boolean = false;
  public filterByServiceName: boolean = false;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.getSettings();
  }

  public getServices() {
    if (!this.machineNames) {
      alert("Please enter a pipe-delimited list of machine names");
      return;
    }

    this.machineToServices = [];
    this.servicesSet = false;
    this.fetchingServices = false;
    this.lastServiceLoaded = false;
    this.fetchedMachines = 0;

    let machines: string[] = this.machineNames.split('|');
    this.fetchingServices = true;

    for (var i = 0; i < machines.length; i++) {
      this.fetchServices(machines[i]);
    }

  }

  public fetchServices(machineName: string) {
    if (this.machineNames || this.serviceNameFilter) {
      this.setSettings();
    }


    this.http.get<Service[]>('api/Service/' + machineName).subscribe(result => {
      console.log("results for machine " + machineName, result);

      let machineToServices: MachineToServices = new MachineToServices();

      machineToServices.machineName = machineName;
      machineToServices.services = result;

      this.machineToServices.push(machineToServices);

      this.fetchedMachines += 1;

      if (this.fetchedMachines == this.machineNames.split('|').length) {
        this.servicesSet = true;
        this.fetchingServices = false;
        console.log("machine to serviesc:", this.machineToServices);
      }

    })
  }

  public showAdvancedOptions() {
    this.advancedOptionsActive = !this.advancedOptionsActive;
  }

  public filterOnServiceNameEnabled() {
    this.filterByServiceName = !this.filterByServiceName;
    this.setSettings();
  }

  public stopService(service: Service) {

    if (service.state.includes('STOPPED')) {
      alert('cannot kill ' + service.displayName + '. Service is currently stopped');
      return;
    }

    let displayServiceText: string = 'Service Name: ' + service.serviceName + '\r\n' +
      'Display Name: ' + service.serviceName + '\r\n' +
      'State: ' + service.state.substring(0, 10) + '\r\n' +
      'PID: ' + service.pid + '\r\n' +
      'Machine: ' + service.machineName + '\r\n' + '\r\n'
      + 'Kill Service?';

    let killService: boolean = confirm(displayServiceText);
    if (killService) {
      this.killService(service);
    }

  }

  public killService(service: Service) {
    service.state = 'attempting to kill service...'

    this.http.put<KillResponse>('api/Service/' + service.machineName, { pid: service.pid }).subscribe(result => {
      if (result.message.includes('SUCCESS: The process with PID ' + service.pid + ' has been terminated.')) {
        service.state = 'STOPPED';
      }

      alert(result.message);
    });
  }

  public onKeydown(event:any) {
    if (event.key === "Enter") {
      this.filterByServiceName = true;
      this.setSettings();
    }
  }

  public setSettings() {

    try {
      this.http.post('api/Settings/', { settingsBlob: JSON.stringify({ machineNames: this.machineNames, serviceNameFilter: this.serviceNameFilter })}).subscribe(result => {
        console.log("set settings result", result);
      })
    } catch (e) {
      console.log(e);
    }


  }

  public getSettings() {

    try {
      this.http.get<SettingsBlob>('api/Settings/').subscribe(result => {
        console.log('get settings', result);

        if (result && result.settingsBlob) {
          let settingsBlob: any = JSON.parse(result.settingsBlob);

          this.machineNames = settingsBlob.machineNames;
          this.serviceNameFilter = settingsBlob.serviceNameFilter;
        }

      })
    } catch (e) {
      console.log(e);
    }


  }

  public classLayout() {
    return 'col-sm-' + (12 / this.machineToServices.length) +' body-content';
  }

  public getMaxTableCellWidth() {
    return (window.innerWidth / this.machineToServices.length - 50).toString();
  }

}

