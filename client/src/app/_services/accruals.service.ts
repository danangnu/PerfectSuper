import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Accrual } from '../_models/accrual';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AccrualsService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAccrual() {
    return this.http.get<Accrual[]>(this.baseUrl + 'accrual');
  }

  saveAccrual(rdata: string) {
    return this.http.post(this.baseUrl + 'accrual/addcsv', rdata).pipe(
      map((user) => {

      })
    );
  }

  saveAccrual2(rdata: string) {
    return this.http.post(this.baseUrl + 'accrual/addcsv/offline', rdata);
  }

  clearData() {
    return this.http.delete(this.baseUrl + 'accrual');
  }
}
