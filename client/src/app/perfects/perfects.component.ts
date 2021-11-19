import { Template } from '@angular/compiler/src/render3/r3_ast';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Accrual } from '../_models/accrual';
import { Datarecords } from '../_models/datarecords';
import { AccrualsService } from '../_services/accruals.service';
import { DatarecordsService } from '../_services/datarecords.service';
import { ExportExcelService } from '../_services/export-excel.service';

@Component({
  selector: 'app-perfects',
  templateUrl: './perfects.component.html',
  styleUrls: ['./perfects.component.css'],
})
export class PerfectsComponent implements OnInit {
  perfectForm = new FormGroup({
    myobtype: new FormControl('', Validators.required),
  });
  accrual: Accrual[];
  accrualError: Accrual[];
  drecsEmp: Datarecords;
  drecsAcr: Datarecords;
  dataForExcel = [];
  file: any;
  value: any;
  bsModelRef: BsModalRef;

  constructor(
    private accrualService: AccrualsService,
    private datarecordsService: DatarecordsService,
    private ete: ExportExcelService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.loadAccrual();
    this.getUpdatedEmp();
    this.getUpdatedAcr();
  }

  onClickEmp() {
    this.drecsAcr.myob === 0 ? this.uploadEmployee() : this.uploadEmployee2();
  }

  onClickAcr() {
    this.drecsAcr.myob === 0 ? this.uploadAccrual() : this.uploadAccrual2();
  }

  getUpdatedEmp() {
    this.datarecordsService.getRecords('employees').subscribe((response) => {
      this.drecsEmp = response;
    });
  }

  getUpdatedAcr() {
    this.datarecordsService.getRecords('accrual').subscribe((response) => {
      this.drecsAcr = response;
    });
  }

  getErrors(): number {
    let error = 0;
    this.accrual.forEach((acc) => {
      if (
        !(
          (acc.empCategory.toString() === 'Temporary' &&
            acc.empStatus === 'Casual' &&
            acc.customList2 === 'FTM') ||
          (acc.empCategory.toString() === 'Permanent' &&
            acc.empStatus === 'Full Time' &&
            acc.customList2 === 'Salaries') ||
          (acc.empCategory.toString() === 'Permanent' &&
            acc.empStatus === 'Part Time' &&
            acc.customList2 === 'Salaries')
        )
      ) {
        error += 1;
        this.accrualError.push(acc);
      }
    });
    return error;
  }

  getHeaders() {
    const headers: string[] = [];
    if (this.accrual) {
      this.accrual.forEach((value) => {
        Object.keys(value).forEach((key) => {
          if (!headers.find((header) => header === key)) {
            headers.push(key);
          }
        });
      });
    }
    return headers;
  }

  loadAccrual() {
    this.accrualService.getAccrual().subscribe((accrual) => {
      this.accrual = accrual;
    });
  }

  fileChanged(e) {
    this.file = e.target.files[0];
  }

  uploadEmployee() {}

  uploadEmployee2() {}

  uploadAccrual() {}

  uploadAccrual2() {}

  onItemChange(value) {
    this.value = value;
  }

  openModal(template: TemplateRef<any>) {
    this.bsModelRef = this.modalService.show(template);
    this.accrual = this.accrualError;
  }

  closeModal() {
    this.bsModelRef.hide();
  }

  exportExcel() {
    this.accrual.forEach((row: any) => {
      this.dataForExcel.push(Object.values(row));
    });

    const reportData = {
      title: 'PerfectSuper',
      data: this.dataForExcel,
      headers: Object.keys(this.accrual[0]),
    };

    this.ete.exportExcel(reportData);
  }

  deleteData() {}
}
