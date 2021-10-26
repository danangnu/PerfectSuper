import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Datarecords } from '../_models/datarecords';

@Injectable({
  providedIn: 'root'
})
export class DatarecordsService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getRecords(id: string) {
    return this.http.get<Datarecords>(this.baseUrl + 'datarecords/' + id);
  }

  updateRecords(id: string, value: number) {
    return this.http.post(this.baseUrl + 'datarecords/updateDesc/' + id + '/' + value, null).pipe(
      map((user) => {

      })
    );
  }
}
