using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoldiersInfo.Controllers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using SoldiersInfo.Models;
using System.IO;
using System.Globalization;

namespace SoldiersInfo.Controllers
{
    public class Printing
    {
        static string fontname = "ARIALUNI.TTF";
        static string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"fonts\", fontname);
        static BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED); // take Unicode font
        static Font font11n = new Font(bf, 11, Font.NORMAL, BaseColor.BLACK);
        static Font font12bi = new Font(bf, 12, Font.BOLDITALIC, BaseColor.BLACK);
        static Font font13n = new Font(bf, 13, Font.NORMAL, BaseColor.BLACK);
        static Font font13b = new Font(bf, 13, Font.BOLD, BaseColor.BLACK);
        static Font font13i = new Font(bf, 13, Font.ITALIC, BaseColor.BLACK);
        static Font font14b = new Font(bf, 14, Font.BOLD, BaseColor.BLACK);

        static float documentMarginLeft = Utilities.MillimetersToPoints(30);
        static float documentMarginRight = Utilities.MillimetersToPoints(15);
        static float documentMarginTopBottom = Utilities.MillimetersToPoints(20);
        static float documentContentWidth = PageSize.A4.Width - documentMarginLeft - documentMarginRight;

        static float[] widthsOfColumnsTable = { 50f, 200f, 60f, 120f, 120f, 100f, 100f };

        // Giá trị mặc định
        static string defaultCompany = "A1";
        static String[] defaultSigner = { "PHÓ TRƯỞNG PHÒNG", "Thượng tá", "Phạm Văn Vĩnh" };

        // National Brand
        static DocObject nationalBrandObj = new DocObject(
            new float[] { documentMarginLeft + PageSize.A4.Width / 2 - 100f, PageSize.A4.Width - documentMarginRight },
            new String[] { "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", "Độc lập - Tự do - Hạnh phúc" },
            new Font[] { font13b, font14b },
            Element.ALIGN_CENTER, 66f, 0
            );

        // Office
        static DocObject officeObj = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width / 2 - 10f },
            new String[] { "CÔNG AN TỈNH KHÁNH HÒA", "PHÒNG TỔ CHỨC CÁN BỘ" },
            new Font[] { font13n, font13b },
            Element.ALIGN_CENTER, 50f, 0);

        // Decision No
        static DocObject decisionNoObj = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width / 2 - 30f },
            null,
            new Font[] { font13n },
            Element.ALIGN_CENTER, 0, 0);

        // Place and Date
        static DocObject placeAndDateObj = new DocObject(
            new float[] { documentMarginLeft + PageSize.A4.Width / 2 - 100f, PageSize.A4.Width - documentMarginRight },
            null,
            new Font[] { font13i },
            Element.ALIGN_CENTER, 0, 0);

        // Type and Principle Content
        static DocObject typeAndMainContentObj = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width - documentMarginRight },
            new String[] { "THÔNG BÁO", "V/v thực hiện chế độ phụ cấp thâm niên nghề trong CAND" },
            new Font[] { font14b },
            Element.ALIGN_CENTER, 40f, 0);

        // Base Document
        static DocObject baseDocsObj = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width - documentMarginRight },
            new String[] {
                "- Căn cứ Thông tư số 54/2014/TT-BCA ngày 30/10/2014 của Bộ Công An quy định về chế độ phụ cấp thâm niên nghề trong Công an nhân dân;",
                "- Căn cứ hồ sơ cán bộ Phòng Tổ chức cán bộ đang quản lý;"
            },
            new Font[] { font13n },
            Element.ALIGN_JUSTIFIED, 0, 20f);


        // Annoucement
        static DocObject annoucementObj_beforeTable = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width - documentMarginRight },
            null,
            new Font[] { font13n },
            0, 0, 20f);
        static DocObject annoucementObj_afterTable = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width - documentMarginRight },
            null,
            new Font[] { font13n },
            0, 0, 20f);
        static String[] annoucementText_single_form = {
                "- Phòng tổ chức cán bộ thông báo việc thực hiện chế độ phụ cấp thâm niên nghề trong CAND đối với đồng chí sau, hiện đang công tác tại đơn vị ",
                "Cứ thêm 01 năm (đủ 12 tháng) công tác trong CAND, kể từ thời điểm hưởng khởi điểm, đồng chí trên được tính thêm 01% phụ cấp thâm niên nghề.",
                "Xin thông báo Phòng PH41, đơn vị ",
                " và đồng chí trên biết, thực hiện./."
            };
        static String[] annoucementText_multiple_form = {
                "- Phòng tổ chức cán bộ thông báo việc thực hiện chế độ phụ cấp thâm niên nghề trong CAND đối với đồn các đồng chí sau, hiện đang công tác tại đơn vị ",
                "Cứ thêm 01 năm (đủ 12 tháng) công tác trong CAND, kể từ thời điểm hưởng khởi điểm, các đồng chí trên được tính thêm 01% phụ cấp thâm niên nghề.",
                "Xin thông báo Phòng PH41, đơn vị ",
                " và các đồng chí trên biết, thực hiện./."
            };

        static DocObject soldiersTable = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width - documentMarginRight },
            null,
            null,
            0, 0, 0);

        // Right and Sigature
        static DocObject signatureObj = new DocObject(
            new float[] { documentMarginLeft + PageSize.A4.Width / 2 - 50f, PageSize.A4.Width - documentMarginRight },
            null,
            new Font[] { font13b },
            Element.ALIGN_CENTER, 0, 0);

        // Receiver
        static DocObject receiverObj = new DocObject(
            new float[] { documentMarginLeft, documentMarginLeft + PageSize.A4.Width / 2 - 50f },
            null,
            null,
            Element.ALIGN_LEFT, 0, 0);

        // Note
        static DocObject noteObj = new DocObject(
            new float[] { documentMarginLeft, PageSize.A4.Width - documentMarginRight },
            new String[] {
                "Ghi chú:",
                " Nếu có vấn đề chưa rõ, các đơn vị và CBCS liên hệ Đội Chính sách và Bảo hiểm – Phòng PX13 (SĐT: 3691.397) để phối hợp giải quyết./."
            },
            new Font[] { font13b, font13n },
            0, 0, 0);

        static public MemoryStream streamPrint(string decisionString, DateTime date, String[,] list_soldiers, String[] signer, String[] receiver, String company)
        {
            // xử lý dữ liệu đầu vào
            int list_length = list_soldiers.GetLength(0);
            decisionNoObj.text = fix_DecisionNo(decisionString);

            placeAndDateObj.text = new String[] { fix_PlaceAndDate(date) };

            company.Trim();


            if (list_length < 2)
            {
                annoucementObj_beforeTable.text = fix_Annoucement(1, company, true);
                annoucementObj_afterTable.text = fix_Annoucement(1, company, false);
            }
            else
            {
                annoucementObj_beforeTable.text = fix_Annoucement(2, company, true);
                annoucementObj_afterTable.text = fix_Annoucement(2, company, false);
            }

            receiverObj.text = fix_Receiver(receiver);
            receiverObj.font = fix_FontReceiver(receiverObj.text);

            signatureObj.text = fix_Signer(signer);

            String[,] listSoldiersInStringArray = new String[list_length, 5]; // take 5 properties
            listSoldiersInStringArray = list_soldiers;

            MemoryStream stream = new MemoryStream();
            Document document = new Document(PageSize.A4, documentMarginLeft, documentMarginRight, documentMarginTopBottom, documentMarginTopBottom);

            try
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);
                pdfWriter.CloseStream = false;

                document.Open();

                PdfContentByte content = pdfWriter.DirectContent;

                // nationalBrand, office, decideNo, placeAndDate, typeAndPrincContent, baseDocument, single_form, note, rightAndSignature, receiver

                DocObject[] printObjs = {
                    nationalBrandObj, officeObj, decisionNoObj, placeAndDateObj, typeAndMainContentObj, baseDocsObj, annoucementObj_beforeTable,
                    soldiersTable,
                    annoucementObj_afterTable, signatureObj, receiverObj, noteObj };

                DocObject[] headerObjs = { officeObj, nationalBrandObj, decisionNoObj, placeAndDateObj };
                DocObject[] mainContent =
                    { typeAndMainContentObj, baseDocsObj, annoucementObj_beforeTable, soldiersTable, annoucementObj_afterTable };

                DocObject[] receiverAndSignObjs = { receiverObj, signatureObj };
                DocObject[] footerObjects = { noteObj };

                //ColumnText[] listComponents;// = new ColumnText[];
                //listComponents = /*test(content);*/creatListColumnText(content, printObjs, listSoldiers);
                //for (int i = 0; i < headerObjs.Length; i++)
                //{
                //    //if (listComponents[i] != null)
                //    //document.Add(new Paragraph("aa", font11n));
                //    listComponents[i].Go();
                //}
                // a rectangle is drawn by 4 param, as order: llx, lly, upx, upy; llx - lower left x, lly - lower left y, urx - upper right x, ury - upper right y
                int litsObjLength = 8;
                Paragraph[] list_paragraph_Content = new Paragraph[litsObjLength];
                Paragraph headerParagraph = paragraph_Table_Content(headerObjs);
                Paragraph receiverAndSignParagraph = paragraph_Table_Content_HasReceiver(receiverAndSignObjs);
                Paragraph noteParagraph = paragraph_NoteObject(noteObj);
                list_paragraph_Content[0] = headerParagraph;
                list_paragraph_Content[1 + mainContent.Length] = receiverAndSignParagraph;
                list_paragraph_Content[1 + mainContent.Length + 1] = noteParagraph;

                for (int i = 0; i < litsObjLength; i++)
                    switch (i)
                    {
                        default:
                            list_paragraph_Content[i] = paragraph_each_DocObject(mainContent[i - 1], true);
                            break;
                        case 7:
                            list_paragraph_Content[i] = noteParagraph;
                            break;
                        case 6:
                            list_paragraph_Content[i] = receiverAndSignParagraph;
                            break;
                        case 4:
                            list_paragraph_Content[i] = paragraph_Table_Soldiers(listSoldiersInStringArray);
                            break;
                        case 2:
                            list_paragraph_Content[i] = paragraph_each_DocObject(mainContent[i - 1], false);
                            break;
                        case 0:
                            list_paragraph_Content[i] = headerParagraph;
                            break;

                    }
                for (int i = 0; i < list_paragraph_Content.Length; i++)
                    document.Add(list_paragraph_Content[i]);


            }
            catch (DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }

            document.Close();

            stream.Flush(); //Always catches me out
            stream.Position = 0; //Adobe Reader required
            return stream;
        }

        static String[] fix_DecisionNo(string decisionString)
        {
            int decisionNo;
            bool couldConvertToInt;
            string[] result = new string[1];

            if (!String.IsNullOrEmpty(decisionString))
            {
                couldConvertToInt = int.TryParse(decisionString, out decisionNo);
                if (couldConvertToInt)
                    result[0] = "Số: " + decisionNo.ToString() + "/TB-PX13(CSBH)";
                else
                    result[0] = "Số: " + decisionString.Trim();
            }
            else
                result[0] = "Số: 0/TB-PX13(CSBH)";
            return result;
        }
        static String fix_PlaceAndDate(DateTime source)
        {
            return "Khánh Hòa, ngày " + source.Day + " tháng " + source.Month + " năm " + source.Year;
        }
        static String[] fix_Annoucement(int total, string company, bool beforeObj)
        {
            String[] result;
            if (beforeObj)
            {
                result = new string[1];
                if (total < 2)
                    result[0] = annoucementText_single_form[0] + company + ":";
                else result[0] = annoucementText_multiple_form[0] + company + ":";

            }
            else
            {
                result = new string[2];
                if (total < 2)
                {
                    result[0] = annoucementText_single_form[1];
                    result[1] = annoucementText_single_form[2] + company + annoucementText_single_form[3];
                }
                else
                {
                    result[0] = annoucementText_multiple_form[1];
                    result[1] = annoucementText_multiple_form[2] + company + annoucementText_multiple_form[3];
                }
            }
            return result;
        }
        static String[] fix_Signer(String[] source)
        {
            byte position = 0;
            byte armyRank = 1;
            byte signerName = 2;

            String positionString, armyRankString, signerNameString;
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            for (int i = 0; i < source.Length; i++)
                source[i] = source[i].Trim();

            if (String.IsNullOrEmpty(source[position]))
                positionString = defaultSigner[position];
            else positionString = source[position].ToUpper(); // vị trí: UPPERCASE

            if (String.IsNullOrEmpty(source[armyRank]))
                armyRankString = defaultSigner[armyRank];
            else
            {
                armyRankString = source[armyRank].ToLower();
                string allButFirstLetter = armyRankString.Substring(1);
                string firstLetter = armyRankString.Substring(0, 1);
                armyRankString = firstLetter.ToUpper() + allButFirstLetter; // Quân hàm: Sentence case
            }

            if (String.IsNullOrEmpty(source[signerName]))
                signerNameString = defaultSigner[signerName];
            else signerNameString = myTI.ToTitleCase(source[signerName]); // Tên : Title case

            String[] result;
            if (source[position].Equals("TRƯỞNG PHÒNG"))
                result = new String[] { positionString, "\n", "\n", "\n", "\n", "\n", armyRankString + " " + signerNameString };
            else
                result = new String[] { "KT. TRƯỞNG PHÒNG", positionString, "\n", "\n", "\n", "\n", "\n", armyRankString + " " + signerNameString };

            return result;
        }
        static Font[] fix_FontReceiver(String[] source)
        {
            int length = source.Length;
            Font[] result = new Font[length];
            result[0] = font12bi;

            for (int i = 1; i < length; i++)
                result[i] = font11n;

            return result;
        }
        static String[] fix_Receiver(String[] source)
        {
            int length = source.Length;
            for (int i = 0; i < length; i++)
                source[i].Trim();
            String[] result = new String[length + 1];

            result[0] = "Nơi nhận:";
            for (int i = 0; i < length - 1; i++)
                result[i + 1] = source[i] + ";";
            result[length] = source[length - 1] + ".";
            return result;
        }

        static Paragraph paragraph_Table_Content(DocObject[] obj)
        {
            Paragraph result = new Paragraph();

            PdfPTable contentInTableform = new PdfPTable(2);
            float _1stColumnWidth = obj[0].box[1] - obj[0].box[0];
            float _2ndColumnWidth = PageSize.A4.Width - _1stColumnWidth - documentMarginLeft - documentMarginRight;
            float[] tableWidths = { _1stColumnWidth, _2ndColumnWidth };
            contentInTableform.TotalWidth = PageSize.A4.Width - documentMarginLeft - documentMarginRight;
            contentInTableform.SetWidths(tableWidths);
            contentInTableform.LockedWidth = true;

            for (int k = 0; k < obj.Length; k++)
            {
                PdfPCell cell = new PdfPCell();
                cell.Border = Rectangle.NO_BORDER;
                cell.AddElement(paragraph_each_DocObject(obj[k], false));

                contentInTableform.AddCell(cell);
            }
            contentInTableform.SpacingAfter = 20f;
            result.Add(contentInTableform);

            return result;
        }
        static Paragraph paragraph_Table_Content_HasReceiver(DocObject[] obj)
        { // chỉ riêng cho receiver và sign
            Paragraph result = new Paragraph();

            PdfPTable contentInTableform = new PdfPTable(2);
            float _1stColumnWidth = obj[0].box[1] - obj[0].box[0];
            float _2ndColumnWidth = PageSize.A4.Width - _1stColumnWidth;
            float[] tableWidths = { _1stColumnWidth, _2ndColumnWidth };
            contentInTableform.TotalWidth = PageSize.A4.Width - documentMarginLeft - documentMarginRight;
            contentInTableform.SetWidths(tableWidths);
            contentInTableform.LockedWidth = true;

            Paragraph paragraphReceiver = paragraph_Receiver(obj[0]); // receiver : obj[0]
            PdfPCell cellReceiver = new PdfPCell();
            cellReceiver.Border = Rectangle.NO_BORDER;
            cellReceiver.AddElement(paragraphReceiver);
            contentInTableform.AddCell(cellReceiver);

            // sign : obj[1]
            PdfPCell cell = new PdfPCell();
            cell.Border = Rectangle.NO_BORDER;
            cell.AddElement(paragraph_each_DocObject(obj[1], true));
            contentInTableform.AddCell(cell);
            contentInTableform.SpacingAfter = 20f;
            result.Add(contentInTableform);

            return result;
        }
        static Paragraph paragraph_each_DocObject(DocObject source, bool hasAfterSpace)
        {
            Paragraph result = new Paragraph();
            for (int i = 0; i < source.text.Length; i++)
            {
                Paragraph paragraph;
                if (source.font.Length > 1) // mọi chữ có cùng 1 font.
                    paragraph = paragraph_Text(source.text[i], source.font[i], source.align, source.firstIndent);
                else
                    paragraph = paragraph_Text(source.text[i], source.font[0], source.align, source.firstIndent);
                result.Add(paragraph);
            }

            if (source.lineSeperatorPercent != 0)
                result.Add(chunk_Lineseparator(source.lineSeperatorPercent));

            if (hasAfterSpace)
                result.SpacingAfter = 20f;
            return result;
        }
        static Paragraph paragraph_NoteObject(DocObject source)
        {
            Paragraph result = new Paragraph();
            Chunk a = new Chunk(source.text[0], source.font[0]);
            Phrase b = new Phrase(source.text[1], source.font[1]);
            result.Add(a);
            result.Add(b);
            return result;
        }
        static Paragraph paragraph_Receiver(DocObject receiver)
        {
            Paragraph result = new Paragraph();
            Phrase legend = new Phrase(receiver.text[0], receiver.font[0]);
            List list_Receiver = new List(List.UNORDERED, 10f);
            for (int i = 1; i < receiver.text.Length; i++) // phần tử đầu tiên là "Nơi nhận"
                list_Receiver.Add(new ListItem(receiver.text[i], receiver.font[i]));
            result.Add(legend);
            result.Add(list_Receiver);
            return result;
        }
        static Paragraph paragraph_Text(String text, Font font, int align, float firstIndent)
        {
            Paragraph result = new Paragraph(text, font);
            result.Alignment = align;
            result.FirstLineIndent = firstIndent;
            return result;
        }
        static Chunk chunk_Lineseparator(float percent)
        {
            LineSeparator line = new LineSeparator();
            line.Percentage = percent;
            return new Chunk(line);
        }

        static Paragraph paragraph_Table_Soldiers(String[,] listSoldiers)
        {
            Paragraph result = new Paragraph();

            PdfPTable soldierTable = table_Soldiers_To_Add(listSoldiers);
            Paragraph tableSoldierPara = new Paragraph();
            result.Add(soldierTable);
            result.SpacingAfter = 30f;
            return result;
        }
        static PdfPTable table_Soldiers_To_Add(String[,] listSoldiers)
        {
            PdfPTable table = new PdfPTable(7);

            table.TotalWidth = PageSize.A4.Width - documentMarginLeft - documentMarginRight;
            table.SetWidths(widthsOfColumnsTable);
            table.LockedWidth = true;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            String[] headers =
            {
                "STT", "Họ tên", "Năm sinh", "Ngày vào CAND", "Thời điểm hưởng khởi điểm", "Mức hưởng khởi điểm", "Ghi chú"
               //null, h = 0,    h = 1,      h = 2,           h = 3,                      , null,                 h = 4
            };

            for (int i = 0; i < headers.Length; i++)
            {
                PdfPCell cell = cell_To_Add(headers[i], font13b, Element.ALIGN_MIDDLE, Element.ALIGN_CENTER);
                table.AddCell(cell);
            }

            for (int k = 0; k < listSoldiers.GetLength(0); k++)
            {
                int serial = k + 1; // Điền thông tin số thứ tự
                PdfPCell serialCell = cell_To_Add(serial.ToString() + ".", font13n, Element.ALIGN_MIDDLE, Element.ALIGN_CENTER);
                table.AddCell(serialCell);

                PdfPCell nameCell = cell_To_Add(listSoldiers[k, 0], font13n, Element.ALIGN_MIDDLE, Element.ALIGN_LEFT);
                table.AddCell(nameCell);

                for (int h = 1; h < listSoldiers.GetLength(1); h++)
                {
                    PdfPCell cell = cell_To_Add(listSoldiers[k, h], font13n, Element.ALIGN_MIDDLE, Element.ALIGN_CENTER);
                    if (h == 4) // Lúc này, listSoldiers[k, h] chứa ghi chú, cần điền thông tin về mức % tăng trước.
                    {
                        PdfPCell percentCell = cell_To_Add("05%", font13n, Element.ALIGN_MIDDLE, Element.ALIGN_CENTER);
                        table.AddCell(percentCell);
                    }
                    table.AddCell(cell);
                }
            }
            return table;
        }
        static PdfPCell cell_To_Add(string content, Font font, int alignVertical, int alignHorizontal)
        {
            Phrase pharseToAdd = new Phrase(content, font);
            PdfPCell cell = new PdfPCell(pharseToAdd);
            cell.VerticalAlignment = alignVertical;
            cell.HorizontalAlignment = alignHorizontal;
            cell.PaddingBottom = 5f;
            return cell;
        }
    }

    public struct DocObject
    {
        public const byte _BORDERLR = 0;
        public const byte _TEXT = 1;
        public const byte _FONT = 2;
        public const byte _ALIGN = 3;
        public const byte _LINESPRTOR = 4;
        public const byte _FRSTINDENT = 5;

        public float[] box { get; set; }
        public String[] text { get; set; }
        public Font[] font { get; set; }
        public int align { get; set; }
        public float lineSeperatorPercent { get; set; }
        public float firstIndent { get; set; }

        public DocObject(float[] box, String[] text, Font[] font, int align, float lineSeperatorPercent, float firstIndent)
        {
            this.box = box;
            this.text = text;
            this.font = font;
            this.align = align;
            this.lineSeperatorPercent = lineSeperatorPercent;
            this.firstIndent = firstIndent;
        }
    }
}

/*
 * có dùng code từ các trang sau:
 *  https://msdn.microsoft.com/en-us/library/system.globalization.textinfo.totitlecase%28v=vs.110%29.aspx
 *  http://www.mikesdotnetting.com/article/89/itextsharp-page-layout-with-columns
 *  http://www.mikesdotnetting.com/Article/81/iTextSharp-Working-with-Fonts
 *  http://www.mikesdotnetting.com/Article/82/iTextSharp-Adding-Text-with-Chunks-Phrases-and-Paragraphs
 *  http://www.mikesdotnetting.com/article/83/lists-with-itextsharp
 *  http://www.mikesdotnetting.com/Article/86/iTextSharp-Introducing-Tables
 */




