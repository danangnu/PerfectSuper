import { Injectable } from '@angular/core';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { Accrual } from '../_models/accrual';

@Injectable({
  providedIn: 'root',
})
export class ExportExcelService {
  constructor() {}

  // tslint:disable-next-line: typedef
  exportExcel(excelData1, excelData2) {
    // Title, Header & Data
    const title = excelData1.title;
    const header: Accrual[] = excelData1.headers;
    const data: Accrual[] = excelData1.data;

    // Create a workbook with a worksheet
    // tslint:disable-next-line: prefer-const
    let workbook = new Workbook();
    // tslint:disable-next-line: prefer-const
    let worksheet = workbook.addWorksheet('Employee Details');

    // Add Row and formatting
    worksheet.mergeCells('C1', 'F4');
    // tslint:disable-next-line: prefer-const
    let titleRow = worksheet.getCell('C1');
    titleRow.value = 'Employee Details';
    titleRow.font = {
      name: 'Calibri',
      size: 16,
      underline: 'single',
      bold: true,
      color: { argb: '0085A3' },
    };
    titleRow.alignment = { vertical: 'middle', horizontal: 'center' };

    // Date
    worksheet.mergeCells('G1:H4');
    // tslint:disable-next-line: prefer-const
    let d = new Date();
    // tslint:disable-next-line: prefer-const
    if (d.getMonth() < 2) {
    }
    // tslint:disable-next-line: prefer-const
    let date =
      d.getDate() +
      '-' +
      ('0' + (d.getMonth() + 1)).slice(-2) +
      '-' +
      d.getFullYear();
    // tslint:disable-next-line: prefer-const
    let dateCell = worksheet.getCell('G1');
    dateCell.value = date;
    dateCell.font = {
      name: 'Calibri',
      size: 12,
      bold: true,
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
        bgColor: { argb: '' },
      };
      cell.font = {
        bold: true,
        color: { argb: 'FFFFFF' },
        size: 12,
      };
    });

    // Adding Data with Conditional Formatting
    // tslint:disable-next-line: no-shadowed-variable
    data.forEach((d) => {
      // tslint:disable-next-line: prefer-const
      let row = worksheet.addRow(d);

      // tslint:disable-next-line: prefer-const
      let sales = row.getCell(2);
      let color = 'FF99FF99';
      if (sales.value === null) {
        color = 'FF9999';
      }

      sales.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: color },
      };
    });

    worksheet.getColumn(1).width = 16;
    worksheet.getColumn(2).width = 25;
    worksheet.getColumn(3).width = 20;
    worksheet.getColumn(4).width = 25;
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

    // worksheet.getColumn(8).numFmt = '#,##0.00;[Red]-#,##0.00';
    // worksheet.getColumn(9).numFmt = '#,##0.00;[Red]-#,##0.00';
    // worksheet.getColumn(10).numFmt = '#,##0.00;[Red]-#,##0.00';
    // worksheet.getColumn(11).numFmt = '#,##0.00;[Red]-#,##0.00';
    // worksheet.getColumn(12).numFmt = '#,##0.00;[Red]-#,##0.00';
    // worksheet.getColumn(13).numFmt = '#,##0.00;[Red]-#,##0.00';
    // worksheet.getColumn(14).numFmt = '#,##0.00;[Red]-#,##0.00';

    // Footer Row
    // tslint:disable-next-line: prefer-const
    let footerRow = worksheet.addRow(['Perfect Super at ' + date]);
    footerRow.getCell(1).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFB050' },
    };
    // Merge Cells
    worksheet.mergeCells(`A${footerRow.number}:F${footerRow.number}`);

    const title2 = excelData2.title;
    const header2: Accrual[] = excelData2.headers;
    const data2: Accrual[] = excelData2.data;

    // tslint:disable-next-line: prefer-const
    let worksheet2 = workbook.addWorksheet('Super Perfect');

    // Add Row and formatting
    worksheet2.mergeCells('C1', 'F4');
    // tslint:disable-next-line: prefer-const
    let titleRow2 = worksheet2.getCell('C1');
    titleRow2.value = title2;
    titleRow2.font = {
      name: 'Calibri',
      size: 16,
      underline: 'single',
      bold: true,
      color: { argb: '0085A3' },
    };
    titleRow2.alignment = { vertical: 'middle', horizontal: 'center' };

    // Date
    worksheet2.mergeCells('G1:H4');
    // tslint:disable-next-line: prefer-const
    let e = new Date();
    // tslint:disable-next-line: prefer-const
    if (e.getMonth() < 2) {
    }
    // tslint:disable-next-line: prefer-const
    let date2 =
      e.getDate() +
      '-' +
      ('0' + (e.getMonth() + 1)).slice(-2) +
      '-' +
      e.getFullYear();
    // tslint:disable-next-line: prefer-const
    let dateCell2 = worksheet2.getCell('G1');
    dateCell2.value = date2;
    dateCell2.font = {
      name: 'Calibri',
      size: 12,
      bold: true,
    };
    dateCell2.alignment = { vertical: 'middle', horizontal: 'center' };

    // Blank Row
    worksheet2.addRow([]);

    // Adding Header Row
    // tslint:disable-next-line: prefer-const
    let headerRow2 = worksheet2.addRow(header2);
    // tslint:disable-next-line: variable-name
    headerRow2.eachCell((cell, number) => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '4167B8' },
        bgColor: { argb: '' },
      };
      cell.font = {
        bold: true,
        color: { argb: 'FFFFFF' },
        size: 12,
      };
    });

    // Adding Data with Conditional Formatting
    // tslint:disable-next-line: no-shadowed-variable
    data2.forEach((e) => {
      // tslint:disable-next-line: prefer-const
      let row2 = worksheet2.addRow(e);

      // tslint:disable-next-line: prefer-const
      let sales2 = row2.getCell(6);
      let color = 'FF99FF99';
      if (sales2.value === null) {
        color = 'FF9999';
      }

      sales2.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: color },
      };
    });

    worksheet2.getColumn(1).width = 16;
    worksheet2.getColumn(2).width = 9;
    worksheet2.getColumn(3).width = 21;
    worksheet2.getColumn(4).width = 19;
    worksheet2.getColumn(5).width = 21;
    worksheet2.getColumn(6).width = 11;
    worksheet2.getColumn(8).width = 16;
    worksheet2.getColumn(10).width = 15;
    worksheet2.getColumn(11).width = 12;
    worksheet2.getColumn(12).width = 13;
    worksheet2.getColumn(13).width = 20;
    worksheet2.getColumn(14).width = 21;
    worksheet2.getColumn(15).width = 20;
    worksheet2.getColumn(19).width = 17;
    worksheet2.getColumn(21).width = 10;
    worksheet2.getColumn(22).width = 11;
    worksheet2.getColumn(23).width = 11;
    worksheet2.getColumn(24).width = 14;
    worksheet2.getColumn(25).width = 13;
    worksheet2.getColumn(26).width = 11;
    worksheet2.getColumn(27).width = 21;
    worksheet2.getColumn(28).width = 30;
    worksheet2.getColumn(29).width = 30;
    worksheet2.getColumn(30).width = 30;
    worksheet2.getColumn(31).width = 30;
    worksheet2.addRow([]);

    worksheet2.getColumn(8).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet2.getColumn(9).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet2.getColumn(10).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet2.getColumn(11).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet2.getColumn(12).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet2.getColumn(13).numFmt = '#,##0.00;[Red]-#,##0.00';
    worksheet2.getColumn(14).numFmt = '#,##0.00;[Red]-#,##0.00';

    // Footer Row
    // tslint:disable-next-line: prefer-const
    let footerRow2 = worksheet2.addRow(['Perfect Super at ' + date2]);
    footerRow2.getCell(1).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFB050' },
    };
    worksheet2.getCell(`H${footerRow2.number - 1}`).value = {
      formula: `SUM(H7:H${footerRow2.number - 2})`,
      date1904: false,
    };
    worksheet2.getCell(`I${footerRow2.number - 1}`).value = {
      formula: `SUM(I7:I${footerRow2.number - 2})`,
      date1904: false,
    };
    worksheet2.getCell(`J${footerRow2.number - 1}`).value = {
      formula: `SUM(J7:J${footerRow2.number - 2})`,
      date1904: false,
    };
    worksheet2.getCell(`K${footerRow2.number - 1}`).value = {
      formula: `SUM(K7:K${footerRow2.number - 2})`,
      date1904: false,
    };
    worksheet2.getCell(`L${footerRow2.number - 1}`).value = {
      formula: `SUM(L7:L${footerRow2.number - 2})`,
      date1904: false,
    };
    worksheet2.getCell(`M${footerRow2.number - 1}`).value = {
      formula: `SUM(M7:M${footerRow2.number - 2})`,
      date1904: false,
    };
    worksheet2.getCell(`N${footerRow2.number - 1}`).value = {
      formula: `SUM(N7:N${footerRow2.number - 2})`,
      date1904: false,
    };
    // Merge Cells
    worksheet2.mergeCells(`A${footerRow2.number}:F${footerRow2.number}`);

    // Generate & Save Excel File
    // tslint:disable-next-line: no-shadowed-variable
    workbook.xlsx.writeBuffer().then((data3) => {
      // tslint:disable-next-line: prefer-const
      let blob = new Blob([data3], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });
      fs.saveAs(blob, title + '.xlsx');
    });
  }
}
