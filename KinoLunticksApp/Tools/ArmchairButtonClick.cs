using System.Windows;
using System.Windows.Controls;

namespace KinoLunticksApp.Tools
{
    public class ArmchairButtonClick
    {
        public Button Button { get; set; }

        public ArmchairButtonClick(Button button)
        {
            Button = button;
        }

        public void Handle()
        {
            // Логика обработки щелчка кнопки кресла
            if (Button.Style == (Style)Application.Current.Resources["GreenArmchairButtonStyle"])
            {
                Button.Style = (Style)Application.Current.Resources["PurpleArmchairButtonStyle"];
                // Добавьте выбранное место в список выбранных мест и увеличьте сумму билетов
            }
            else if (Button.Style == (Style)Application.Current.Resources["PurpleArmchairButtonStyle"])
            {
                Button.Style = (Style)Application.Current.Resources["GreenArmchairButtonStyle"];
                // Удалите выбранное место из списка выбранных мест и уменьшите сумму билетов
            }
        }
    }
}
