import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PerfectsComponent } from './perfects/perfects.component';

const routes: Routes = [
  {path: '', component: PerfectsComponent},
  {path: 'perfects', component: PerfectsComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
