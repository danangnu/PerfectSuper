import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import {NgxSpinnerModule} from 'ngx-spinner';
import { ToastrModule } from 'ngx-toastr';
import { Ng2SmartTableModule } from 'ng2-smart-table';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { PerfectsComponent } from './perfects/perfects.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    PerfectsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    NgxSpinnerModule,
    Ng2SmartTableModule,
    ToastrModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
