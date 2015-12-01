using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Overheads.Core;
using System.Windows;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Overheads
{
    public static class PrintableDocuments
    {
        public static FlowDocument IndexDocument(List<Book> books)
        {
            FlowDocument doc = new FlowDocument();

            Table table = new Table();

            int numberOfColumns = 3;
            for (int x = 0; x < numberOfColumns; x++)
            {
                table.Columns.Add(new TableColumn());

                if (x % 2 == 0)
                    table.Columns[x].Background = Brushes.Beige;
                else
                    table.Columns[x].Background = Brushes.LightSteelBlue;

                if(x < 2)
                    table.Columns[x].Width = new GridLength(1, GridUnitType.Star);
                else
                    table.Columns[x].Width = new GridLength(4, GridUnitType.Star);
            }

            table.RowGroups.Add(new TableRowGroup());

            // Create Title
            table.RowGroups[0].Rows.Add(new TableRow());

            TableRow currentRow = table.RowGroups[0].Rows[0];

            currentRow.Background = Brushes.Silver;
            currentRow.FontSize = 20;
            currentRow.FontWeight = FontWeights.Bold;

            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Overheads Index"))));
            currentRow.Cells[0].ColumnSpan = 3;
            /////////////////////////////

            // Create Header
            table.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table.RowGroups[0].Rows[1];

            currentRow.FontSize = 12;
            currentRow.FontWeight = FontWeights.Bold;

            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Book"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Number"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Title"))));

            currentRow.Cells[0].FontWeight = FontWeights.Bold;
            //////////////////////////////

            // Add Data
            int currentIndex = 2;
            foreach(Book book in books)
            {
                foreach(SearchSong song in book.Songs)
                {
                    table.RowGroups[0].Rows.Add(new TableRow());
                    currentRow = table.RowGroups[0].Rows[currentIndex];
                
                    currentRow.FontSize = 12;
                    currentRow.FontWeight = FontWeights.Normal;

                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run(book.Title))));
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run(song.Number))));
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run(song.Title))));

                    currentIndex++;
                }
            }
            ///////////////////////////////

            // Create Footer
            table.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table.RowGroups[0].Rows[currentIndex];

            currentRow.Background = Brushes.LightGray;
            currentRow.FontSize = 18;
            currentRow.FontWeight = FontWeights.Normal;

            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(DateTime.Now.ToString("M/d/yyyy")))));

            doc.Blocks.Add(table);

            return doc;
        }

        public static FlowDocument SongDocument(Song song)
        {
            FlowDocument doc = new FlowDocument();

            Paragraph title = new Paragraph(new Run(song.Title));
            title.FontSize = 25;

            doc.Blocks.Add(title);

            if (string.IsNullOrEmpty(song.Subtitle) == false)
            {
                Paragraph subtitle = new Paragraph(new Run(song.Subtitle));
                doc.Blocks.Add(subtitle);
            }

            Song doubledSong = new Song(song.SongText, song.Key);

            foreach(Verse verse in doubledSong.Verses)
            {
                for(int lineIndex = 0; lineIndex < verse.AllLines.Count; lineIndex++)
                {
                    if (lineIndex + 1 < verse.AllLines.Count)
                    {
                        Line currentLine = verse.AllLines[lineIndex];
                        Line nextLine = verse.AllLines[lineIndex + 1];

                        if (currentLine.IsNotText)
                        {
                            currentLine.Text = currentLine.Text.PadRight(Convert.ToInt32(Math.Ceiling(nextLine.Text.Length * 1.4)));
                        }

                        if (currentLine.IsNotText == nextLine.IsNotText)
                        {
                            currentLine.Text += " " + nextLine.Text;
                            verse.AllLines.Remove(nextLine);
                        }
                        else if (lineIndex + 2 < verse.AllLines.Count)
                        {
                            Line lineAfterNext = verse.AllLines[lineIndex + 2];

                            if(currentLine.IsNotText == lineAfterNext.IsNotText)
                            {
                                currentLine.Text += " " + lineAfterNext.Text;
                                verse.AllLines.Remove(lineAfterNext);
                            }
                        }
                    }
                }
            }

            foreach(Verse verse in doubledSong.Verses)
            {
                foreach(Line line in verse.AllLines)
                {
                    Paragraph currentLine = new Paragraph(new Run(line.Text));

                    if(line.IsNotText)
                    {
                        currentLine.FontSize = 12;
                        currentLine.FontStyle = FontStyles.Italic;
                        currentLine.Foreground = Brushes.Blue;
                        currentLine.FontFamily = new FontFamily("Consolas");
                        currentLine.LineHeight = 5;
                    }
                    else
                    {
                        currentLine.FontFamily = new FontFamily("Segoe UI");
                        currentLine.FontSize = 22;
                        currentLine.LineHeight = 5;
                    }

                    doc.Blocks.Add(currentLine);
                }
                
                Paragraph space = new Paragraph(new Run(""));
                doc.Blocks.Add(space);
            }

            return doc;
        }
    }
}
