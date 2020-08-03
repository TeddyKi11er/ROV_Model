using System;
using UnityEngine;
using ServDynamicsModuleice;
using Newtonsoft.Json;
using Service;
using System.IO;

namespace DynamicsModule
{
    /// <summary>
    /// Класс PA используется для расчета перемещения
    /// и положения подводного аппарата. В программе не может
    /// быть больше одного экземпляпа данного класса
    /// </summary>
    public class PA
    {
        private Carcass carcass;
        // маршевые двигатели
        private WMA wma1;
        private WMA wma2;
        // двигатели под корпусом 
        private WMA wma3;
        private WMA wma4;

        // служебные поля класса

        // главный вектор сил
        private Vector3 mainF;
        // главный вектор моментов
        private Vector3 mainM;
        // векто момента силы архимеда
        private Vector3 MFa;
        // скорость в связанной системе координат
        private Vector3 speed;
        private Vector3 angSpeed;
        // углы, на которые повернут аппарат
        private Vector3 ang;

        // поле реализует паттерн singleton
        private static PA singleton;

        /// <summary>
        /// возвращает ссылку на объект класса PA, реализует паттерн singleton
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PA Instans(String path = "Config.json")
        {
            if (path == null)
            {
                throw new Exception("Не введен путь к Config");
            }
            if (singleton == null)
            {
                singleton = new PA(path);
            }
            return singleton;
        }
        // приватный конструктор
        private PA(String path)
        {
            if (!File.Exists("config.json")) {
                throw new Exception("отвутствует файл конфигурации");
            }

            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            
            carcass = new Carcass(config);

            wma1 = new WMA(config.maxThrust, config.posEngine1, 1);
            wma2 = new WMA(config.maxThrust, config.posEngine2, 2);
            wma3 = new WMA(config.maxThrust, config.posEngine3, 3);
            wma4 = new WMA(config.maxThrust, config.posEngine4, 4);

            mainF = carcass.Fa + carcass.P;
            mainM = new Vector3(carcass.MFa.x, carcass.MFa.y, carcass.MFa.z);
            MFa = new Vector3(carcass.MFa.x, carcass.MFa.y, carcass.MFa.z);
            speed = new Vector3(0, 0, 0);
            angSpeed = new Vector3(0, 0, 0);
            ang = new Vector3(0, 0, 0);
            
        }
        // приватный конструктор копирования, не реализован 
        private PA(PA val) { }


        /// <summary>
        /// метод считает смещение аппарата, возвращаяет вектор, на 
        /// который переместился аппарат
        /// </summary>
        /// <returns></returns>
        public Vector3 displacement()
        {
            // увеличим вектор скорости на значение, рассчтанное в 
            // прошлом цыкле
            updateSpeed();
            // сгенерируем матрицу крылова, чтобы довернуть вектор скорости
            // до полусвязанной системы координат, передадим углы поворота 
            // с минусом, тк поворачиваем из связанной в полусвязанную
            double[,] wingKrylov = setKrylov(-ang);
            Vector3 turnSpeed = move(speed, wingKrylov);
            // посчтитаем перемещение за время между вызовами метода
            Vector3 rezalt = turnSpeed * Time.deltaTime;
            // посчитаем новое значение F

            // довернем вектор веса и силы архимеда до связанной системы координат
            // для этого сгенерируем новую матриуц крылова
            wingKrylov = setKrylov(ang);
            // довернем силу архимеда
            Vector3 turnFa = move(carcass.Fa, wingKrylov);
            // довернем вес
            Vector3 turnP = move(carcass.P, wingKrylov);
            // посчтаем новый момент силы архимеда, потому, что это удобно сделать
            MFa = Vector3.Cross(turnFa, carcass.centrVolume);
            // сложим вектор силы архимеда, веса, тяги двигателей и 
            // гидродинамические силы сопротивления
            mainF = turnFa + turnP + wma1.getF() + wma2.getF() +
                    wma3.getF() + wma4.getF() + hydrodynamicF();
            return rezalt;
        }

        /// <summary>
        /// метод возвращающий углы поворота аппарата относительно полусвязанной
        /// системы координат
        /// </summary>
        /// <returns></returns>
        public Vector3 angleRotation()
        {
            // увеличим угловую скорость на значения посчитанные в прошлом цикле
            updateAngSpeed();
            // сгенерируем матрицу крылова, чтобы довернуть векто угловой скорости до 
            // полусвязанной системы координат
            double[,] wingKrylov = setKrylov(-ang);
            Vector3 turnAngSpeed = move(angSpeed, wingKrylov);
            // обновим углы поворота 
            ang += turnAngSpeed * Time.deltaTime;
            // посчитаем новый момент M
            mainM = MFa + wma1.getM() + wma2.getM() + wma3.getM() + wma4.getM() +
                    hydrodynamicM();
            return new Vector3(ang.x, ang.y, ang.z);
        }

        // считает матрицу поворота для переданных вектором углов 
        private double[,] setKrylov(Vector3 wing)
        {
            double[,] wingKrylova = new double[3, 3];
            wingKrylova[0, 0] = Math.Cos(wing[0]) * Math.Cos(wing[2]) + Math.Sin(wing[0]) *
                                Math.Sin(wing[1]) * Math.Sin(wing[2]);
            wingKrylova[0, 1] = -Math.Cos(wing[0]) * Math.Sin(wing[2]) + Math.Sin(wing[0]) *
                                Math.Sin(wing[1]) * Math.Cos(wing[2]);
            wingKrylova[0, 2] = Math.Sin(wing[0]) * Math.Cos(wing[1]);

            wingKrylova[1, 0] = Math.Cos(wing[1]) * Math.Sin(wing[2]);
            wingKrylova[1, 1] = Math.Cos(wing[1]) * Math.Cos(wing[2]);
            wingKrylova[1, 2] = -Math.Sin(wing[1]);

            wingKrylova[2, 0] = -Math.Sin(wing[0]) * Math.Cos(wing[2]) + Math.Cos(wing[0]) *
                                Math.Sin(wing[1]) * Math.Sin(wing[2]);
            wingKrylova[2, 2] = Math.Sin(wing[0]) * Math.Sin(wing[2]) + Math.Cos(wing[0]) *
                                Math.Sin(wing[1]) * Math.Cos(wing[2]);
            wingKrylova[2, 1] = Math.Cos(wing[0]) * Math.Cos(wing[1]);

            return wingKrylova;
        }

        // возвращает вектор повернутый на углы крылова
        private Vector3 move(Vector3 val, double[,] wingKrylov)
        {
            Vector3 turn = new Vector3(0, 0, 0);
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    turn[i] += (float)wingKrylov[i, j] * val[j];
                }
            }
            return turn;
        }

        // метод, возвращающий силу гидродинамического сопротивления
        private Vector3 hydrodynamicF()
        {
            Vector3 rezalt = new Vector3(0, 0, 0);
            Vector3 hydrodynamiСoef = carcass.hydrodynamiСoef;
            Vector3 sMidelya = carcass.sMidelya;
            for (int i = 0; i < 3; ++i)
            {
                if (speed[i] > 0)
                {
                    rezalt[i] = (float)(- 1000 * hydrodynamiСoef[i] * Math.Pow(speed[i], 2) *
                                sMidelya[i] / 2);
                }
                else
                {
                    rezalt[i] = (float)(- 1000 * hydrodynamiСoef[i] * Math.Pow(speed[i], 2) *
                                sMidelya[i] / 2);
                }
            }
            return rezalt;
        }

        // метод возвращающий момент гидродинамических сил
        private Vector3 hydrodynamicM()
        {
            Vector3 rezalt = new Vector3(0, 0, 0);
            Vector3 hydrodynamiСoef = carcass.hydrodynamiСoef;
            Vector3 dimensions = new Vector3((float)carcass.height, (float)carcass.length, (float)carcass.width);
            Vector3 sMidelya = carcass.sMidelya;
            for (int i = 0; i < 3; ++i)
            {
                if (angSpeed[i] > 0)
                {
                    rezalt[i] = (float)(- hydrodynamiСoef[i] * Math.Pow(angSpeed[i], 2) *
                                Math.Pow(dimensions[i], 3) * sMidelya[i] / 27);
                }
                else
                {
                    rezalt[i] = (float)(- hydrodynamiСoef[i] * Math.Pow(angSpeed[i], 2) *
                                Math.Pow(dimensions[i], 3) * sMidelya[i] / 27);
                }
            }
            return rezalt;
        }
        // увеличивает скорость на значение рассчитанное в предвдущей итерации
        private void updateSpeed()
        {
            speed = speed + (mainF * Time.deltaTime / (float)carcass.m);
        }

        // увеличивает угловую скорость на значения рассчитанные в предыдущей итерации
        private void updateAngSpeed()
        {
            for (int i = 0; i < 3; ++i)
            {
                angSpeed[i] += mainM[i] * Time.deltaTime / carcass.inercApparat[i];
            }
        }


        /// <summary>
        /// Метод отвечающий за движение вдоль оси марша, предполагается,
        /// что 1 - движение вперед, -1 - движение назад, 0 - постепенное 
        /// торможение (нет управляющего сигнала). Будьте внимательны, 
        /// при отсутствии нажатия на клавишу, всем методы движениея должны быть
        /// вызваны с параметром 0 или без параметра!
        /// </summary>
        /// <param name="signal"></param>
        public void march(double signal = 0)
        {
            wma1.setValF(signal);
            wma2.setValF(signal);
        }

        /// <summary>
        /// Метод, отвечающий за лаговое движене, в данной конфигурации, невозможное.
        /// Будьте внимательны, при отсутствии нажатия на клавишу, 
        /// всем методы движениея должны быть вызваны с параметром 0 или без параметра!
        /// </summary>
        /// <param name="signal"></param>
        public void lag(double signal = 0) { }

        /// <summary>
        /// Метод отвечающий за движение вдоль оси глубины, предполагается,
        /// что 1 - всплытие, -1 - движение погружение, 0 - постепенное 
        /// торможение (нет управляющего сигнала). Будьте внимательны, 
        /// при отсутствии нажатия на клавишу, всем методы движениея должны быть
        /// вызваны с параметром 0 или без параметра!
        /// </summary>
        /// <param name="signal"></param>
        public void depth(double signal = 0)
        {
            wma3.setValF(signal);
            wma4.setValF(signal);
        }

        /// <summary>
        /// Метод отвечающий за поворот на угол курса, предполагается,
        /// что 1 - поворот по часовой, -1 - поворот против часовой, 0 - постепенное 
        /// торможение (нет управляющего сигнала). Будьте внимательны, 
        /// при отсутствии нажатия на клавишу, всем методы движениея должны быть
        /// вызваны с параметром 0 или без параметра!
        /// </summary>
        /// <param name="signal1"></param>
        /// <param name="signal2"></param>
        public void course(double signal1 = 0, double signal2 = 0)
        {
            wma1.setValF(signal1);
            wma2.setValF(signal2);
        }

        /// <summary>
        /// Метод отвечающий за поворот на угол дефферента, предполагается,
        /// что 1 - поворот по часовой, -1 - поворот против часовой, 0 - постепенное 
        /// торможение (нет управляющего сигнала). Будьте внимательны, 
        /// при отсутствии нажатия на клавишу, всем методы движениея должны быть
        /// вызваны с параметром 0 или без параметра! 
        /// </summary>
        /// <param name="signal1"></param>
        /// <param name="signal2"></param>
        public void defferent(double signal1 = 0, double signal2 = 0)
        {
            wma3.setValF(signal1);
            wma4.setValF(signal2);
        }

        /// <summary>
        /// Поскольку компоновка аппарата не допускает крен в чистом виде, 
        /// принято решение не реализовывать этот тип движения.
        /// Будьте внимательны, при отсутсвии нажатия на клавишу, всем методы движения
        /// должны быть вызваны с параметром 0 или без параметра!
        /// </summary>
        /// <param name="signal1"></param>
        /// <param name="signal2"></param>
        public void roll(double signal1 = 0, double signal2 = 0) { }
    }
}
