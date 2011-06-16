using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace SubsMuxer {

	public class DataGridViewProgressBarColumn : DataGridViewColumn {
		public DataGridViewProgressBarColumn() : base(new DataGridViewProgressBarCell()) {
		}

		public override DataGridViewCell CellTemplate {
			get { return base.CellTemplate; }
			set {
				// Ensure that the cell used for the template is a ProgressCell.
				if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewProgressBarCell))) {
					throw new InvalidCastException("Must be a DataGridViewProgressBarCell");
				}
				base.CellTemplate = value;
			}
		}

		public class DataGridViewProgressBarCell : DataGridViewImageCell {
			protected override object GetFormattedValue(object value, int rowIndex,	ref DataGridViewCellStyle cellStyle, 
				TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context) {
				Bitmap bmp = new Bitmap(this.Size.Width, this.Size.Height);
				Rectangle rc = Rectangle.Empty;
				rc.Size = bmp.Size;

				Color clrOne = Color.LightBlue;
				Color clrTwo = Color.DarkBlue;

				using (Graphics gfx = Graphics.FromImage(bmp))
				using (Brush b = new LinearGradientBrush(rc, clrOne, clrTwo, LinearGradientMode.Vertical)) {
					gfx.Clear(Color.White);
					// Percentage.
					int percentage = 0;

					if (this.Value != null)
						int.TryParse(this.Value.ToString(), out percentage);
					string text = percentage.ToString() + "%";

					// Get width and height of text.
					Font font = new Font("Tahoma", 10, FontStyle.Regular);
					int width = (int)gfx.MeasureString(text, font).Width;
					int height = (int)gfx.MeasureString(text, font).Height;

					// Draw pile.
					gfx.DrawRectangle(Pens.Black, 2, 2, this.Size.Width - 9, this.Size.Height - 6);
					gfx.FillRectangle(b, 3, 3, (int)(this.Size.Width - 10) * percentage / 100, (int)this.Size.Height - 7);

					RectangleF rect = new RectangleF(0, 0, bmp.Width, bmp.Height);
					StringFormat sf = new StringFormat();
					sf.Alignment = StringAlignment.Center;
					gfx.DrawString(text, font, Brushes.Black, rect, sf);
				}
				return bmp;
			}
		}
	}
}