import { stringify } from '@angular/compiler/src/util';
import { Injectable } from '@angular/core';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class ExportExcelService {

  constructor() { }

  // tslint:disable-next-line: typedef
  exportExcel(excelData) {

    // Title, Header & Data
    const title = excelData.title;
    const header = excelData.headers;
    const data = excelData.data;

    // Create a workbook with a worksheet
    // tslint:disable-next-line: prefer-const
    let workbook = new Workbook();
    // tslint:disable-next-line: prefer-const
    let worksheet = workbook.addWorksheet('Super Perfect');


    // Add Row and formatting
    worksheet.mergeCells('C1', 'F4');
    // tslint:disable-next-line: prefer-const
    let titleRow = worksheet.getCell('C1');
    titleRow.value = title;
    titleRow.font = {
      name: 'Calibri',
      size: 16,
      underline: 'single',
      bold: true,
      color: { argb: '0085A3' }
    };
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };

    // Date
    worksheet.mergeCells('G1:H4');
    // tslint:disable-next-line: prefer-const
    let d = new Date();
    // tslint:disable-next-line: prefer-const
    if (d.getMonth() < 2)
    {

    }
    // tslint:disable-next-line: prefer-const
    let date = d.getDate() + '-' + ('0' + (d.getMonth() + 1)).slice(-2) + '-' + d.getFullYear();
    // tslint:disable-next-line: prefer-const
    let dateCell = worksheet.getCell('G1');
    dateCell.value = date;
    dateCell.font = {
      name: 'Calibri',
      size: 12,
      bold: true
    };
    dateCell.alignment = { vertical: 'middle', horizontal: 'center' };

    // Blank Row
    worksheet.addRow([]);

    // Adding Header Row
    // tslint:disable-next-line: prefer-const
    let headerRow = worksheet.addRow(header);
    // tslint:disable-next-line: variable-name
    headerRow.eachCell((cell, number) => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '4167B8' },
        bgColor: { argb: '' }
      };
      cell.font = {
        bold: true,
        color: { argb: 'FFFFFF' },
        size: 12
      };
    });

    // Adding Data with Conditional Formatting
    // tslint:disable-next-line: no-shadowed-variable
    data.forEach( d => {
      // tslint:disable-next-line: prefer-const
      let row = worksheet.addRow(d);

      // tslint:disable-next-line: prefer-const
      let sales = row.getCell(6);
      let color = 'FF99FF99';
      if (sales.value === null) {
        color = 'FF9999';
      }

      sales.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: color }
      };
    }
    );

    worksheet.getColumn(1).width = 16;
    worksheet.getColumn(2).width = 9;
    worksheet.getColumn(3).width = 17;
    worksheet.getColumn(4).width = 19;
    worksheet.getColumn(5).width = 21;
    worksheet.getColumn(6).width = 11;
    worksheet.getColumn(8).width = 16;
    worksheet.getColumn(10).width = 15;
    worksheet.getColumn(11).width = 12;
    worksheet.getColumn(12).width = 13;
    worksheet.getColumn(13).width = 20;
    worksheet.getColumn(14).width = 21;
    worksheet.getColumn(15).width = 20;
    worksheet.getColumn(19).width = 17;
    worksheet.getColumn(21).width = 10;
    worksheet.getColumn(22).width = 11;
    worksheet.getColumn(23).width = 11;
    worksheet.getColumn(24).width = 14;
    worksheet.getColumn(25).width = 13;
    worksheet.getColumn(26).width = 11;
    worksheet.getColumn(27).width = 21;
    worksheet.getColumn(28).width = 12;
    worksheet.getColumn(29).width = 14;
    worksheet.addRow([]);

    worksheet.getColumn(8).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet.getColumn(9).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet.getColumn(10).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet.getColumn(11).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet.getColumn(12).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet.getColumn(13).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet.getColumn(14).numFmt = '#,##0.00;[Red]-#,##0.00';

    // Footer Row
    // tslint:disable-next-line: prefer-const
    let footerRow = worksheet.addRow(['Perfect Super at ' + date]);
    footerRow.getCell(1).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFB050' }
    };
    worksheet.getCell(`H${footerRow.number - 1}`).value = { formula: `SUM(H7:H${footerRow.number - 2})`, date1904: false };
    worksheet.getCell(`I${footerRow.number - 1}`).value = { formula: `SUM(I7:I${footerRow.number - 2})`, date1904: false };
    worksheet.getCell(`J${footerRow.number - 1}`).value = { formula: `SUM(J7:J${footerRow.number - 2})`, date1904: false };
    worksheet.getCell(`K${footerRow.number - 1}`).value = { formula: `SUM(K7:K${footerRow.number - 2})`, date1904: false };
    worksheet.getCell(`L${footerRow.number - 1}`).value = { formula: `SUM(L7:L${footerRow.number - 2})`, date1904: false };
    worksheet.getCell(`M${footerRow.number - 1}`).value = { formula: `SUM(M7:M${footerRow.number - 2})`, date1904: false };
    worksheet.getCell(`N${footerRow.number - 1}`).value = { formula: `SUM(N7:N${footerRow.number - 2})`, date1904: false };
    // Merge Cells
    worksheet.mergeCells(`A${footerRow.number}:F${footerRow.number}`);

    // Generate & Save Excel File
    // tslint:disable-next-line: no-shadowed-variable
    workbook.xlsx.writeBuffer().then((data) => {
      // tslint:disable-next-line: prefer-const
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      fs.saveAs(blob, title + '.xlsx');
    });

  }
}
