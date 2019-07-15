﻿using System;
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
using VMSpc.XmlFileManagers;
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.CustomComponents;
using VMSpc.DevHelpers;
using System.Timers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.ParamDataManager;
using System.Globalization;

namespace VMSpc.Panels
{
    /// <summary>
    /// base class of all panel elements
    /// </summary>
    public abstract class VPanel
    {
        public char cID;
        private MainWindow mainWindow;
        protected PanelSettings panelSettings;
        public Border border;
        public VMSCanvas canvas;
        
        private int resizeType;

        public double leftLimit;
        public double topLimit;
        public double rightLimit;
        public double bottomLimit;

        public bool isMoving, isResizing;

        private VMSDialog dlgWindow;

        public VPanel(MainWindow mainWindow, PanelSettings panelSettings)
        {
            this.mainWindow = mainWindow;
            this.panelSettings = panelSettings;
            isMoving = false;
            isResizing = false;
            resizeType = RESIZE_NONE;
            dlgWindow = null;
            InitLimits();
            border = new Border() { BorderThickness = new Thickness(5) };
            canvas = new VMSCanvas(mainWindow, border, panelSettings);
            border.Child = canvas;
            GenerateEventHandlers();
            Init();
        }


        protected virtual void Init()
        { 
            canvas.Background = new SolidColorBrush(panelSettings.backgroundColor);
        }

        ~VPanel()
        {

        }

        /// <summary>
        /// Returns a call to the constructor of the corresponding Dialog Window for this panel.
        /// </summary>
        protected abstract VMSDialog GenerateDlg();

        private void GenerateEventHandlers()
        {
            border.MouseEnter += OnMouseOverBorder;
            border.MouseLeave += OnMouseLeaveBorder;
            canvas.MouseEnter += OnMouseLeaveBorder;
            canvas.MouseLeave += OnMouseOverBorder;
            canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            dlgWindow = GenerateDlg();
            dlgWindow.Owner = mainWindow;
            if (dlgWindow != null)
            {
                bool? result = dlgWindow.ShowDialog(this);
                if (result == true)
                {
                    GeneratePanel();
                    Init();
                }
            }
        }

        public void OnMouseOverBorder(object sender, MouseEventArgs e)
        {
            if (isMoving) return;
            DetermineCursor(e.GetPosition(mainWindow));
        }

        private void DetermineCursor(Point pos)
        {
            if (pos.X <= (Canvas.GetLeft(border) + 10) && pos.Y <= (Canvas.GetTop(border) + 10))
            {
                Mouse.OverrideCursor = Cursors.SizeNWSE;
                resizeType = RESIZE_TOPLEFT;
            }
            else if (pos.X >= (Canvas.GetRight(border) - 10) && pos.Y >= (Canvas.GetBottom(border) - 10))
            {
                Mouse.OverrideCursor = Cursors.SizeNWSE;
                resizeType = RESIZE_BOTTOMRIGHT;
            }
            else if (pos.X <= (Canvas.GetLeft(border) + 10) && pos.Y >= (Canvas.GetBottom(border) - 10))
            {
                Mouse.OverrideCursor = Cursors.SizeNESW;
                resizeType = RESIZE_BOTTOMLEFT;
            }
            else if (pos.X >= (Canvas.GetRight(border) - 10) && pos.Y <= (Canvas.GetTop(border) + 10))
            {
                Mouse.OverrideCursor = Cursors.SizeNESW;
                resizeType = RESIZE_TOPRIGHT;
            }
            else if (pos.X <= Canvas.GetLeft(border) + 10)
            {
                Mouse.OverrideCursor = Cursors.SizeWE;
                resizeType = RESIZE_LEFT;
            }
            else if (pos.X >= Canvas.GetRight(border) - 10)
            {
                Mouse.OverrideCursor = Cursors.SizeWE;
                resizeType = RESIZE_RIGHT;
            }
            else if (pos.Y <= Canvas.GetTop(border) + 10)
            {
                Mouse.OverrideCursor = Cursors.SizeNS;
                resizeType = RESIZE_TOP;
            }
            else if (pos.Y >= Canvas.GetBottom(border) - 10)
            {
                Mouse.OverrideCursor = Cursors.SizeNS;
                resizeType = RESIZE_BOTTOM;
            }
        }

        public void OnMouseLeaveBorder(object sender, MouseEventArgs e)
        {
            if (isResizing) return;
            Mouse.OverrideCursor = Cursors.Arrow;
            resizeType = RESIZE_NONE;
        }

        public void Resize(Point cursorPoint)
        {
            switch (resizeType)
            {
                case RESIZE_LEFT:
                    ResizeLeft(cursorPoint.X);
                    break;
                case RESIZE_RIGHT:
                    ResizeRight(cursorPoint.X);
                    break;
                case RESIZE_TOP:
                    ResizeTop(cursorPoint.Y);
                    break;
                case RESIZE_BOTTOM:
                    ResizeBottom(cursorPoint.Y);
                    break;
                case RESIZE_BOTTOMLEFT:
                    ResizeBottom(cursorPoint.Y);
                    ResizeLeft(cursorPoint.X);
                    break;
                case RESIZE_BOTTOMRIGHT:
                    ResizeBottom(cursorPoint.Y);
                    ResizeRight(cursorPoint.X);
                    break;
                case RESIZE_TOPLEFT:
                    ResizeTop(cursorPoint.Y);
                    ResizeLeft(cursorPoint.X);
                    break;
                case RESIZE_TOPRIGHT:
                    ResizeTop(cursorPoint.Y);
                    ResizeRight(cursorPoint.X);
                    break;
                default:
                    break;
            }
            canvas.ApplyCanvasDimensions();
            GeneratePanel();
        }

        private void ResizeTop(double newTop)
        {
            var oldBottom = Canvas.GetBottom(border);
            Canvas.SetTop(border, newTop);
            border.Height = oldBottom - newTop;
        }

        private void ResizeRight(double newRight)
        {
            border.Width = (newRight - Canvas.GetLeft(border));
            Canvas.SetRight(border, (Canvas.GetLeft(canvas) + border.Width));
        }

        private void ResizeBottom(double newBottom)
        {
            border.Height = (newBottom - Canvas.GetTop(border));
            Canvas.SetBottom(border, (Canvas.GetTop(canvas) + border.Height));
        }
        
        private void ResizeLeft(double newLeft)
        {
            var oldRight = Canvas.GetRight(border);
            Canvas.SetLeft(border, newLeft);
            border.Width = oldRight - newLeft;
        }

        private bool IsWithinBoundary(double targetPosition, double boundaryLimit, double value)
        {
            return ((targetPosition + boundaryLimit >= targetPosition) && (targetPosition - boundaryLimit <= targetPosition));
        }

        /// <summary>
        /// Sets the limits of the panel to the dimensions of the enclosing window
        /// </summary>
        public void InitLimits()
        {
            leftLimit = 0;
            topLimit = 0;
            rightLimit = mainWindow.Width;
            bottomLimit = mainWindow.Height;
        }

        /// <summary>
        /// compares limits against the dimensions of the VPanel parameter. Changes the limits if the dimensions are closer to this panel's current offset
        /// </summary>
        /// <param name="panel"></param>
        public void SetDirectionalLimits(VPanel panel)
        {
            var panelLeft = Canvas.GetLeft(panel.border);
            var panelTop = Canvas.GetTop(panel.border);
            var panelRight = Canvas.GetRight(panel.border);
            var panelBottom = Canvas.GetBottom(panel.border);
            var thisPanelLeft = Canvas.GetLeft(border);
            var thisPanelRight = Canvas.GetRight(border);
            var thisPanelTop = Canvas.GetTop(border);
            var thisPanelBottom = Canvas.GetBottom(border);
            if (CollisionPossible(thisPanelTop, thisPanelBottom, panelTop, panelBottom))
            {
                if ((panelRight > (leftLimit)) && (panelRight <= Canvas.GetLeft(border)))
                    leftLimit = panelRight;
                if ((panelLeft < (rightLimit)) && (panelLeft >= Canvas.GetRight(border)))
                    rightLimit = panelLeft;
            }
            if (CollisionPossible(thisPanelLeft, thisPanelRight, panelLeft, panelRight))
            {
                if ((panelBottom > (topLimit)) && (panelBottom <= Canvas.GetTop(border)))
                    topLimit = panelBottom;
                if ((panelTop < (bottomLimit)) && (panelTop >= Canvas.GetBottom(border)))
                    bottomLimit = panelTop;
            }
        }

        private bool CollisionPossible(double tPanelEdge1, double tPanelEdge2, double nPanelEdge1, double nPanelEdge2)
        {
            if (        //is this panel's lower border (top or left) in between the parameter panel's corresponding edges?
                    (tPanelEdge1 >= nPanelEdge1 && tPanelEdge1 <= nPanelEdge2)
                    ||  //is this panel's upper border (bottom or right) in between the parameter panel's corresponding edges?
                    (tPanelEdge2 <= nPanelEdge2 && tPanelEdge2 >= nPanelEdge1)
                    ||  //is the parameter panel's lower border (top or left) in between this panel's corresponding edges?
                    (nPanelEdge1 >= tPanelEdge1 && nPanelEdge1 <= tPanelEdge2)
                    ||  //is the parameter panel's upper border (bottom or right) in between this panel's corresponding edges?
                    (nPanelEdge2 <= tPanelEdge2 && nPanelEdge2 >= tPanelEdge1)
                )
                return true;
            return false;
        }

        /// <summary>
        /// Determines if there is still space to move between the panel and it's bounding Horizontal or Vertical neighbor
        /// </summary>
        public bool CanMove(int direction, double newVal)
        {
            switch (direction)
            {
                case HORIZONTAL:
                    return (newVal >= leftLimit && (newVal + border.Width) <= rightLimit);
                case VERTICAL:
                    return (newVal >= topLimit && (newVal + border.Height) <= bottomLimit);
                default:
                    return false;
            }
        }

        public void SetVertical(double newTop, Point newCursorPoint)
        {
            double clipSide;
            if (CanMove(VERTICAL, newTop))
            {
                clipSide = GetVerticalClipSide(newTop);
                if (!Double.IsNaN(clipSide))
                    ClipVertical(clipSide);
                else
                {
                    Canvas.SetTop(border, newTop);
                    Canvas.SetBottom(border, newTop + border.Height);
                }
            }
        }

        public void SetHorizontal(double newLeft, Point newCursorPoint)
        {
            double clipSide;
            if (CanMove(HORIZONTAL, newLeft))
            {
                clipSide = GetHorizontalClipSide(newLeft);
                if (!Double.IsNaN(clipSide))
                    ClipHorizontal(clipSide);
                else
                {
                    Canvas.SetLeft(border, newLeft);
                    Canvas.SetRight(border, newLeft + border.Width);
                }
            }
        }

        /// <summary>
        /// Determines which vertical side needs to be clipped to the bounding neighbor.
        /// </summary>
        /// <param name="newTop"></param>
        /// <returns>The side to be clipped. NaN, if no clipping should occur</returns>
        private double GetVerticalClipSide(double newTop)
        {
            double newBottom = newTop + border.Height;
            if (newTop <= (topLimit + 20))
                return UP;
            else if (newBottom >= (bottomLimit - 20))
                return DOWN;
            return Double.NaN;
        }

        /// <summary>
        /// Determines which horizontal side needs to be clipped to the bounding neighbor.
        /// </summary>
        /// <param name="newTop"></param>
        /// <returns>The side to be clipped. NaN, if no clipping should occur</returns>
        private double GetHorizontalClipSide(double newLeft)
        {
            double newRight = newLeft + border.Width;
            if (newLeft <= (leftLimit + 20))
                return LEFT;
            else if (newRight >= (rightLimit - 20))
                return RIGHT;
            return Double.NaN;
        }

        private void ClipVertical(double side)
        {
            if (side == UP)
            {
                Canvas.SetTop(border, topLimit);
                Canvas.SetBottom(border, topLimit + border.Height);
            }
            else if (side == DOWN)
            {
                Canvas.SetTop(border, bottomLimit - border.Height);
                Canvas.SetBottom(border, bottomLimit);
            }
        }

        private void ClipHorizontal(double side)
        {
            if (side == LEFT)
            {
                Canvas.SetLeft(border, leftLimit);
                Canvas.SetRight(border, leftLimit + border.Width);
            }
            else if (side == RIGHT)
            {
                Canvas.SetLeft(border, rightLimit - border.Width);
                Canvas.SetRight(border, rightLimit);
            }
        }

        /// <summary>
        /// Gets the last updated value of the parameter at the specified PID
        /// </summary>
        protected double GetPidValue(ushort pid)
        {
            if (panelSettings.showInMetric)
                return ParamData.parameters[pid].LastMetricValue;
            else
                return ParamData.parameters[pid].LastValue;
        }

        public abstract void GeneratePanel();

        public abstract void UpdatePanel();

        /// <summary>
        /// Assigns the appropriate right and bottom coordinates of an element. The element's width and height must already be set before calling this method
        /// </summary>
        protected void ApplyRightBottomCoords(FrameworkElement element)
        {
            Canvas.SetBottom(element, Canvas.GetTop(element) + element.Height);
            Canvas.SetRight(element, Canvas.GetLeft(element) + element.Width);
        }

        public void SaveSettings()
        {
            panelSettings.rectCord.bottomRightX = (int)Canvas.GetRight(border);
            panelSettings.rectCord.bottomRightY = (int)Canvas.GetBottom(border);
            panelSettings.rectCord.topLeftX = (int)Canvas.GetLeft(border);
            panelSettings.rectCord.topLeftY = (int)Canvas.GetTop(border);
        }

        protected void ScaleText(TextBlock textBlock, double maxWidth, double maxHeight)
        {
            textBlock.FontSize = 12;
            Size size = CalculateStringSize(textBlock);
            while (size.Width > maxWidth || size.Height > maxHeight)
            {
                    textBlock.FontSize--;
                    size = CalculateStringSize(textBlock);
            }
            while (size.Width < (maxWidth) && size.Height < (maxHeight))
            {
                textBlock.FontSize++;
                size = CalculateStringSize(textBlock);
            }
        }

        private Size CalculateStringSize(TextBlock textBlock)
        {
            if (textBlock.Text == "")
                return new Size(0, 0);
            FormattedText text = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);
            return new Size(text.Width, text.Height);
        }
        
        protected void BalanceTextBlocks(dynamic parent)
        {
            double min = Double.MaxValue;

            foreach (var block in parent.Children)
            {
                if (block.GetType().ToString() == "System.Windows.Controls.TextBlock")
                {
                    if (((TextBlock)block).FontSize < min)
                        min = ((TextBlock)block).FontSize;
                }
                else if (block.GetType().ToString() == "System.Windows.Controls.Border")
                {
                    if (((Border)block).Child.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    {
                        TextBlock textBlock = (TextBlock)((Border)block).Child;
                        if (textBlock.FontSize < min)
                            min = textBlock.FontSize;
                    }
                }
            }
            foreach (var block in parent.Children)
            {
                if (block.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    ((TextBlock)block).FontSize = min;
                else if (block.GetType().ToString() == "System.Windows.Controls.Border")
                {
                    if (((Border)block).Child.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    {
                        TextBlock textBlock = (TextBlock)((Border)block).Child;
                        textBlock.FontSize = min;
                    }

                }
            }
        }
    }
}
