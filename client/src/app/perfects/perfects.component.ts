import { Component, OnInit } from '@angular/core';
import { Accrual } from '../_models/accrual';
import { Datarecords } from '../_models/datarecords';
import { AccrualsService } from '../_services/accruals.service';
import { DatarecordsService } from '../_services/datarecords.service';

@Component({
  selector: 'app-perfects',
  templateUrl: './perfects.component.html',
  styleUrls: ['./perfects.component.css']
})
export class PerfectsComponent implements OnInit {
  settings = {
    actions: false,
    pager: { display: false },
    columns: {
      usi: {
        title: 'USI'
      },
      payrollID: {
        title: 'PayrollID'
      },
      memberID: {
        title: 'MemberID'
      },
      lastName: {
        title: 'Last Name'
      },
      firstName: {
        title: 'First Name'
      },
      dob: {
        title: 'DOB'
      },
      gender: {
        title: 'Gender'
      },
      superGuarantee: {
        title: 'SuperGuarantee',
      },
      spouse: {
        title: 'Spouse'
      },
      salarySacrifice: {
        title: 'SalarySacrifice'
      },
      redundancy: {
        title: 'Redundancy'
      },
      productivity: {
        title: 'Productivity'
      },
      employerAdditional: {
        title: 'EmployerAdditional'
      },
      employeeAdditional: {
        title: 'EmployeeAdditional'
      },
      address1: {
        title: 'Address1'
      },
      address2: {
        title: 'Address2'
      },
      address3: {
        title: 'Address3'
      },
      address4: {
        title: 'Address4'
      },
      locality: {
        title: 'Locality'
      },
      state: {
        title: 'State'
      },
      postcode: {
        title: 'Postcode'
      },
      country: {
        title: 'Country'
      },
      tfn: {
        title: 'TFN'
      },
      empStartDate: {
        title: 'EmpStartDate'
      },
      empEndDate: {
        title: 'EmpEndDate'
      },
      empStatus: {
        title: 'EmpStatus'
      },
      email: {
        title: 'Email'
      },
      phone1: {
        title: 'Phone1'
      },
      phone2: {
        title: 'Phone2'
      }
    }
  };
  accrual: Accrual[];
  drecsEmp: Datarecords;
  drecsAcr: Datarecords;
  file: any;

  constructor(private accrualService: AccrualsService,
              private datarecordsService: DatarecordsService) { }

  ngOnInit(): void {
    this.loadAccrual();
    this.getUpdatedEmp();
    this.getUpdatedAcr();
  }

  onClickEmp(){
    this.drecsAcr.myob === 0 ? this.uploadEmployee() : this.uploadEmployee2();
   }

   onClickAcr(){
    this.drecsAcr.myob === 0 ? this.uploadAccrual() : this.uploadAccrual2();
   }

  getUpdatedEmp() {
    this.datarecordsService.getRecords('employees').subscribe(response => {
      this.drecsEmp = response;
    });
  }

  getUpdatedAcr() {
    this.datarecordsService.getRecords('accrual').subscribe(response => {
      this.drecsAcr = response;
    });
  }

  loadAccrual() {
    this.accrualService.getAccrual().subscribe(accrual => {
    this.accrual = accrual;
    });
  }

  fileChanged(e) {
    this.file = e.target.files[0];
  }

  uploadEmployee() {

  }

  uploadEmployee2() {

  }

  uploadAccrual() {

  }

  uploadAccrual2() {

  }

  exportExcel() {

  }

  deleteData() {

  }
}
