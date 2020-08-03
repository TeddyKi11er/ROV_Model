using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Service;
using System.Dynamic;

namespace ServDynamicsModuleice
{
    /// <summary>
    /// Данный класс инкапсулирует данные и методы расчета для
    /// винтоматорного агрегата
    /// </summary>
    internal class WMA
    {
        private double Fmax;
        private Vector3 position;
        private Vector3 deltaF;
        private Vector3 F;
        private Vector3 M;

        public WMA(double Fmax, threeValues position, int number)
        {
            if (Fmax <= 0)
            {
                throw new Exception("ошибка при инициализации двигателя: " + number +
                                    " (неправильная максимальная сила)");
            }
            this.Fmax = Fmax;
            this.position = new Vector3((float)position.x, (float)position.y,
                                        (float)position.z);
            switch (number)
            {
                case 1:
                case 2:
                    this.deltaF = Vector3.right;
                    break;
                case 3:
                case 4:
                    this.deltaF = Vector3.up;
                    break;
                default:
                    throw new Exception("ошибка при инициализации двигателя" +
                                        " неправильный номер " + number);
                    break;
            }
            this.F = Vector3.zero;
            this.M = Vector3.zero;
        }

        public WMA(double Fmax, threeValues position, threeValues direction)
        {
            if (Fmax <= 0)
            {
                throw new Exception("ошибка при инициализации двигателя" +
                                    "(неправильная максимальная сила)");
            }
            this.Fmax = Fmax;
            this.position = new Vector3((float)position.x, (float)position.y,
                                        (float)position.z);
            this.position = new Vector3((float)direction.x, (float)direction.y,
                                        (float)direction.z);
            this.F = Vector3.zero;
            this.M = Vector3.zero;
        }

        /// <summary>
        /// Конструктор копирования, запрещен к использованию 
        /// </summary>
        /// <param name="val"></param>
        private WMA(WMA val) { }


        /// <summary>
        /// Рассчитывает тыгу по сигналу управления, за основу взято 
        /// апериодическое звено T = 0.15 c
        /// </summary>
        /// <param name="signal"></param>
        public void setValF(double signal)
        {
            double lenF = 0;

            // этот if нужен чтобы учесть направление силы тяги
            if (Vector3.Dot(deltaF, F) > 0)
            {
                // реализует апериодическое звено
                lenF = (signal - F.magnitude / Fmax) / 0.15 * Time.deltaTime;
            }
            else
            {
                lenF = (signal + F.magnitude / Fmax) / 0.15 * Time.deltaTime;
            }
            F = F + (deltaF * (float)lenF);
            // если на вход подается линейная функция, стоит проверить 
            // не превысила ли тяга максимальное значение
            if (F.magnitude > Fmax)
            {
                F = F * ((float)Fmax / F.magnitude);
            }
            // посчитаем новый момент двигателя относительно цм
            setM();
        }

        /// <summary>
        /// Считает момент двигателя относительно цм, вызывается
        /// методом setValF, не вижу смысла делать этот метод публичным
        /// </summary>
        private void setM()
        {
            M = Vector3.Cross(F, position);
        }


        /// <summary>
        /// Метод для получения значения вектора силы
        /// </summary>
        /// <returns></returns>
        public Vector3 getF()
        {
            return new Vector3(F.x, F.y, F.z);
        }

        /// <summary>
        /// Метод для получения значения вектора момента
        /// </summary>
        /// <returns></returns>
        public Vector3 getM()
        {
            return new Vector3(M.x, M.y, M.z);
        }
    }
}