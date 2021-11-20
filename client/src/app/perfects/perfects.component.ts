import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Accrual } from '../_models/accrual';
import { Datarecords } from '../_models/datarecords';
import { AccrualsService } from '../_services/accruals.service';
import { ConfirmService } from '../_services/confirm.service';
import { DatarecordsService } from '../_services/datarecords.service';
import { EmployeeService } from '../_services/employee.service';
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
  accrualError: Accrual[] = [];
  drecsEmp: Datarecords;
  drecsAcr: Datarecords;
  dataForExcel = [];
  file: any;
  value: any;
  bsModelRef: BsModalRef;

  constructor(
    private accrualService: AccrualsService,
    private employeeService: EmployeeService,
    private datarecordsService: DatarecordsService,
    private ete: ExportExcelService,
    private modalService: BsModalService,
    private confirmService: ConfirmService,
    private router: Router
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
        if (this.accrualError.length > 0) {
          if (this.accrualError.indexOf(acc) === -1)
            this.accrualError.push(acc);
        } else this.accrualError.push(acc);
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

  async uploadEmployee() {
    const fileReader = new FileReader();
    fileReader.onload = (e) => {
      console.log(fileReader.result);
    };
    fileReader.readAsText(this.file);
    const text = await new Response(this.file).text();
    this.employeeService.saveEmployee(text).subscribe(() => {
      this.postRequest('Employees', this.drecsAcr.myob);
    });
  }

  async uploadEmployee2() {
    const fileReader = new FileReader();
    fileReader.onload = (e) => {
      console.log(fileReader.result);
    };
    fileReader.readAsText(this.file);
    const text = await new Response(this.file).text();
    this.employeeService.saveEmployee2(text).subscribe(() => {
      this.postRequest('Employees', this.drecsAcr.myob);
    });
  }

  async uploadAccrual() {
    const fileReader = new FileReader();
    fileReader.onload = (e) => {
      console.log(fileReader.result);
    };
    fileReader.readAsText(this.file);
    const text = await new Response(this.file).text();
    this.accrualService.saveAccrual(text).subscribe(() => {
      this.postRequest('Accrual', this.drecsAcr.myob);
    });
  }

  async uploadAccrual2() {
    const fileReader = new FileReader();
    fileReader.onload = (e) => {
      console.log(fileReader.result);
    };
    fileReader.readAsText(this.file);
    const text = await new Response(this.file).text();
    this.accrualService.saveAccrual2(text).subscribe(() => {
      this.postRequest('Accrual', this.drecsAcr.myob);
    });
  }

  onItemChange(value) {
    this.value = value;
  }

  postRequest(id, myob: number) {
    this.datarecordsService.updateRecords(id, myob).subscribe(() => {
      //this.toastr.success('Done');
      this.router
        .navigateByUrl('/', { skipLocationChange: true })
        .then(() => this.router.navigate(['/perfects']));
    });
  }

  openModal(template: TemplateRef<any>) {
    this.bsModelRef = this.modalService.show(template);
    this.accrual = this.accrualError;
  }

  closeModal() {
    this.bsModelRef.hide();
  }

  exportExcel() {
    this.accrual.forEach((row) => {
      this.dataForExcel.push(Object.values(row));
    });

    const reportData = {
      title: 'PerfectSuper',
      data: this.dataForExcel,
      headers: Object.keys(this.accrual[0]),
    };

    this.ete.exportExcel(reportData);
  }

  deleteData() {
    this.confirmService
      .confirm('Confirm delete All DATA', 'This cannot be undone')
      .subscribe((result) => {
        if (result) {
          this.accrualService.clearData().subscribe(() => {
            //this.toastr.success('Done');
            this.router
              .navigateByUrl('/', { skipLocationChange: true })
              .then(() => this.router.navigate(['/perfects']));
          });
        }
      });
  }
}
