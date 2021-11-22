import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Accrual } from '../_models/accrual';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AccrualsService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAccrual() {
    return this.http.get<Accrual[]>(this.baseUrl + 'accrual').pipe(
      map((user) => {
        return user;
      })
    );
  }

  getAccrual2() {
    return this.http.get<Accrual[]>(this.baseUrl + 'accrual/f2').pipe(
      map((user) => {
        return user;
      })
    );
  }

  getAccrualExcel() {
    return this.http.get<Accrual[]>(this.baseUrl + 'accrual/excel').pipe(
      map((user) => {
        return user;
      })
    );
  }

  getAccrualExcel2() {
    return this.http.get<Accrual[]>(this.baseUrl + 'accrual/excel2').pipe(
      map((user) => {
        return user;
      })
    );
  }

  saveAccrual(rdata: string) {
    return this.http
      .post(this.baseUrl + 'accrual/addcsv', rdata)
      .pipe(map((user) => {}));
  }

  saveAccrual2(rdata: string) {
    return this.http.post(this.baseUrl + 'accrual/addcsv/offline', rdata);
  }
}
