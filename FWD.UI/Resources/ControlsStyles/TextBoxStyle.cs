using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace FWD.UI.Resources.ControlsStyles
{
	public static class TextBoxStyle
	{
		private static Effect _oldDisplayEffect = new DropShadowEffect
		{
			ShadowDepth = 6,
			Direction = 135,
			Color = Color.FromArgb(166, 241, 211, 68),
			Opacity = 0.35,
			BlurRadius = 2
		};

		private static Color _foregroundColor = Colors.White;

		private static Color _backGroundColor = Colors.Black;

		public static void ApplyOldDisplayStyle(TextBox box)
		{
			box.Foreground = new SolidColorBrush(_foregroundColor);
			box.Background = new SolidColorBrush(_backGroundColor);
			box.FontWeight = FontWeights.Bold;
		}

		public static void ApplyOldDisplayStyle(TextBlock box)
		{
			box.Effect = _oldDisplayEffect;
			box.Foreground = new SolidColorBrush(_foregroundColor);
			box.Background = new SolidColorBrush(_backGroundColor);
		}
	}
}
