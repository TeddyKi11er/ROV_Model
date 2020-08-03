using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Service;
using System.Dynamic;

namespace ServDynamicsModuleice
{
    /// <summary>
    /// Класс инкапсулирующий все характристики корпуса аппарата
    /// и методы работы с ними
    /// </summary>
    internal class Carcass
    {
        private double m_;
        private double length_;
        private double width_;
        private double height_;
        private double V_;

        // сила архимеда, действующая на па
        private Vector3 Fa_;
        // момент силы архимеда
        private Vector3 P_;
        private Vector3 sMidelya_;
        private Vector3 centrVolume_;
        private Vector3 hydrodynamiСoef_;
        private Vector3 inercApparat_;
        // вес подводного аппарата
        private Vector3 MFa_;

        // этот класс, как и его конструктор существуют только потому, что
        // мне не равится, что все данные хранятся в перемешку в классе 
        // Config
        public Carcass(Config config)
        {
            m_ = config.m;
            length_ = config.length;
            width_ = config.width;
            height_ = config.height;
            V_ = length_ * width_ * height_;

            Fa_ = new Vector3(0, (float)(1000 * 9.81 * V_), 0);
            P_ = new Vector3(0, (float)(-m_ * 9.81), 0);

            sMidelya_ = new Vector3((float)config.sMidelya.x, 
                                    (float)config.sMidelya.y,
                                    (float)config.sMidelya.z);
            centrVolume_ = new Vector3((float)config.centrVolume.x, 
                                       (float)config.centrVolume.y,
                                       (float)config.centrVolume.z);
            hydrodynamiСoef_ = new Vector3((float)config.hydrodynamiСoef.x, 
                                           (float)config.hydrodynamiСoef.y,
                                           (float)config.hydrodynamiСoef.z);
            inercApparat_ = new Vector3((float)config.inercApparat.x, 
                                        (float)config.inercApparat.y,
                                        (float)config.inercApparat.z);
            MFa_ = Vector3.Cross(Fa_, centrVolume_);
        }


        // свойства класса, для более естественного использования
        // данных, строго только для чтения
        public double m
        {
            get
            {
                return m_;
            }
        }

        public double length
        {
            get
            {
                return length_;
            }
        }

        public double width
        {
            get
            {
                return width_;
            }
        }

        public double height
        {
            get
            {
                return height_;
            }
        }

        public double V
        {
            get
            {
                return V_;
            }
        }

        public Vector3 Fa
        {
            get
            {
                return new Vector3(Fa_.x, Fa_.y, Fa_.z);
            }
        }

        public Vector3 P
        {
            get
            {
                return new Vector3(P_.x, P_.y, P_.z);
            }

        }

        public Vector3 sMidelya
        {
            get
            {
                return new Vector3(sMidelya_.x, sMidelya_.y,
                                   sMidelya_.z);
            }
        }

        public Vector3 centrVolume
        {
            get
            {
                return new Vector3(centrVolume_.x, centrVolume_.y,
                                   centrVolume_.z);
            }
        }

        public Vector3 hydrodynamiСoef
        {
            get
            {
                return new Vector3(hydrodynamiСoef_.x, hydrodynamiСoef_.y,
                                   hydrodynamiСoef_.z);
            }
        }

        public Vector3 inercApparat
        {
            get
            {
                return new Vector3(inercApparat_.x, inercApparat_.y,
                                   inercApparat_.z);
            }
        }

        public Vector3 MFa
        {
            get
            {
                return new Vector3(MFa_.x, MFa_.y, MFa_.z);
            }
        }
    }
}