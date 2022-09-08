using BerldSweeper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSweeper.Components
{
    public partial class GameSquare
    {
        [Parameter]
        public EventCallback<(MouseEventArgs, Square)> OnClickCallback { get; set; }

        [Parameter]
        public Square? Square { get; set; }

        private DateTime lastMouseDown = DateTime.MinValue;

        private string StyleForSquareContainer()
        {
            string color = "white";

            if (Square is not null)
            {
                if (Square.State == SquareState.Revealed)
                {
                    if (Square.Value is EmptySquare)
                    {
                        color = "whitesmoke";
                    }
                    else if (Square.Value is NumberSquare)
                    {
                        color = "#c4e6f1";
                    }
                    else if (Square.Value is BombSquare)
                    {
                        color = "red";
                    }
                }
                else if (Square.State == SquareState.Flagged)
                {
                    color = "orange";
                }
                else
                {
                    color = "#dfdfdf";
                }
            }

            return $"background: {color}";
        }

        private string ContentForSquare()
        {
            string content = " ";

            if (Square is not null)
            {
                if (Square.State == SquareState.Revealed)
                {
                    if (Square.Value is NumberSquare numberSquare)
                    {
                        content = numberSquare.Number.ToString();
                    }
                    else if (Square.Value is BombSquare)
                    {
                        content = "💣";
                    }
                }
                else if (Square.State == SquareState.Flagged)
                {
                    content = "🚩";
                }
            }

            return content;
        }

        private string StyleForSquareContent()
        {
            string color = "black";
            string fontSize = "19px";
            string marginTop = "1.5px";

            if (Square is not null && Square.Value is NumberSquare numberSquare)
            {
                color = ColorForNumber(numberSquare.Number);
                fontSize = "25px";
                marginTop = "0.5px";
            }

            return $"color: {color}; font-size: {fontSize}; margin-top: {marginTop}";
        }

        private static string ColorForNumber(int number)
            => number switch
            {
                1 => "blue",
                2 => "green",
                3 => "red",
                4 => "darkblue",
                5 => "brown",
                6 => "#39b794",
                7 => "black",
                8 => "gray",
                _ => "white"
            };

        private void HandleMouseDown(MouseEventArgs e)
        {
            lastMouseDown = DateTime.Now;
        }

        private void HandleMouseUp(MouseEventArgs e)
        {
            if (Square is not null)
            {
                if (e.Button == 0)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan timeSinceLastMouseDown = now - lastMouseDown;

                    if (timeSinceLastMouseDown > TimeSpan.FromSeconds(0.5) && timeSinceLastMouseDown < TimeSpan.FromSeconds(2))
                    {
                        e.Button = 2;
                    }
                }

                OnClickCallback.InvokeAsync((e, Square));
            }
        }
    }
}
