using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Int : MonoBehaviour

{
        static double LeftTriangle(Func<double, double> f, double a, double b, int n)
        {
            var h = (b - a) / n;
            var sum = 0d;
            for (var i = 0; i <= n - 1; i++)
            {
                var x = a + i * h;
                sum += f(x);
            }

            var result = h * sum;
            return result;
        }

        static double RightTriangle(Func<double, double> f, double a, double b, int n)
        {
            var h = (b - a) / n;
            var sum = 0d;
            for (var i = 1; i <= n; i++)
            {
                var x = a + i * h;
                sum += f(x);
            }

            var result = h * sum;
            return result;
        }

        static double CentralTriangle(Func<double, double> f, double a, double b, int n)
        {
            var h = (b - a) / n;
            var sum = 0d;
            for (var i = 0; i < n; i++)
            {
                var x = a + i / 2d * h;
                sum += f(x);
            }

            var result = h * sum;
            return result;
        }

        static void Calc(string[] args)
        {
            //локальная функция
            

           

          
        }
    
        void Start()
{
        double f(double x) => x*x ;
        double result = LeftTriangle(f, 5, 6, 1000);
        Debug.Log("Формула левых прямоугольников: " + result);
    }
}
