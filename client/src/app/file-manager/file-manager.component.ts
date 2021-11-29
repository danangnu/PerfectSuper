import { ThrowStmt } from '@angular/compiler';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { NgxSpinnerService } from 'ngx-spinner';
import { environment } from 'src/environments/environment';
import { Accrual } from '../_models/accrual';
import { Datarecords } from '../_models/datarecords';
import { AccrualsService } from '../_services/accruals.service';
import { DatarecordsService } from '../_services/datarecords.service';

@Component({
  selector: 'app-file-manager',
  templateUrl: './file-manager.component.html',
  styleUrls: ['./file-manager.component.css'],
})
export class FileManagerComponent implements OnInit {
  accrual: Accrual[];
  drecsAcr: Datarecords;
  drecsEmp: Datarecords;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  url: string;
  @Output() nameEmitter = new EventEmitter<any>();
  value: any;
  done: boolean;
  count: number;

  constructor(
    private spinnerService: NgxSpinnerService,
    private accrualService: AccrualsService,
    private datarecordsService: DatarecordsService
  ) {
    this.url = this.baseUrl + 'employees/addcsv';
  }

  ngOnInit(): void {
    this.getUpdatedAcr();
    this.initializeUploader();
  }

  hideAlert() {
    this.done = false;
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  getUpdatedAcr() {
    this.datarecordsService.getRecords('accrual').subscribe((response) => {
      this.drecsAcr = response;
    });
  }

  postRequest(id, myob: number) {
    this.datarecordsService.updateRecords(id, myob).subscribe((value) => {
      if (id === 'Employees') this.drecsEmp = value;
      else this.drecsAcr = value;
    });
  }

  onItemChange(value) {
    this.value = value;
    if (value === 0) {
      this.url = this.baseUrl + 'employees/addcsv';
    } else {
      this.url = this.baseUrl + 'employees/addcsv/offline';
    }
  }

  initializeUploader() {
    this.done = false;
    this.count = 0;
    this.uploader = new FileUploader({
      url: this.baseUrl + 'employees/addcsv',
      isHTML5: true,
      allowedMimeType: ['text/plain'],
      removeAfterUpload: true,
      autoUpload: false,
    });

    this.uploader.onProgressAll = () => {
      this.spinnerService.show(undefined, {
        type: 'line-scale-party',
        bdColor: 'rgba(255,255,255,0)',
        color: '#333333',
      });
    };

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onCompleteItem = (item: any, status: any) => {
      this.spinnerService.hide();
      this.count++;
      // console.log('Uploaded File Details:', item?.file?.name);
    };

    this.uploader.onCompleteAll = () => {
      this.postRequest('Employees', this.drecsAcr.myob);
      this.postRequest('Accrual', this.drecsAcr.myob);
      this.accrualService.getAccrual().subscribe((accrual) => {
        this.accrual = accrual;
        const drecsAcr = this.drecsAcr;
        const drecsEmp = this.drecsEmp;
        this.nameEmitter.emit({ make: accrual, name: drecsAcr, sane: drecsEmp });
        // console.log(this.count);
      });
      this.done = true;
      // this.PostData();
    };
  }
}
