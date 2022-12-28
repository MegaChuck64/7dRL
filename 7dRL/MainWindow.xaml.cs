using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _7dRL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainGame game;
        public MainWindow()
        {
            InitializeComponent();
            DayCycle.Time = new TimeOnly(12, 0);
            EventLogger.Init(EventListBox);
            game = new MainGame(this);
            GameGrid.Children.Add(game);
            game.PauseToggled += Game_PauseToggled;
        }

        private void Game_PauseToggled(object? sender, EventArgs e)
        {
            StoreItems.ItemsSource = new List<StoreItem>
            {
                new StoreItem
                {                
                    Name = "Short Sword",
                    Description = "Short sword with 3 ad",
                    Price = 5,
                    Image = Graphics.SpriteToImage(game.Atlas, (2, 28)),
                },
                new StoreItem
                {
                    Name = "Bow",
                    Description = "Bow that shoots for 4 base ad",
                    Price = 7,
                    Image = Graphics.SpriteToImage(game.Atlas, (5, 28)),
                }
            };
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            GameGrid.Children.Remove(game);
            game?.Dispose();
            GameOverGrid.Visibility = Visibility.Hidden;
            PauseMenu.Visibility = Visibility.Hidden;
            EventLogger.Events.Clear();
            EventListBox.Items.Clear();
            EventListBox.UpdateLayout();
            DayCycle.Time = new TimeOnly(12, 0);
            game = new MainGame(this);
            GameGrid.Children.Add(game);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            game.TogglePause();
        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            var item = StoreItems.SelectedItem as StoreItem;
            if (item?.Price <= 10) //todo: can you afford it
            {
                EventLogger.AddEvent("Purchase " + item.Name + " for " + item.Price + " gold", true);
            }
        }
    }
}
