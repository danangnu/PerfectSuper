import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  baseUrl = environment.apiUrl + 'Employees/';

  constructor(private http: HttpClient) { }

  saveEmployee(rdata: string) {
    return this.http.post(this.baseUrl + 'addcsv', rdata).pipe(
      map((user) => {

      })
    );
  }

  saveEmployee2(rdata: string) {
    return this.http.post(this.baseUrl + 'addcsv/offline', rdata).pipe(
      map((user) => {

      })
    );
  }
}
