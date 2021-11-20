import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FileUploader } from 'ng2-file-upload';
import { NgxSpinnerService } from 'ngx-spinner';
import { environment } from 'src/environments/environment';
import { Accrual } from '../_models/accrual';

@Component({
  selector: 'app-file-manager',
  templateUrl: './file-manager.component.html',
  styleUrls: ['./file-manager.component.css'],
})
export class FileManagerComponent implements OnInit {
  @Input() accrual: Accrual;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;

  constructor(
    private spinnerService: NgxSpinnerService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  initializeUploader() {
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
      console.log('Uploaded File Details:', item?.file?.name);
    };
  }
}
