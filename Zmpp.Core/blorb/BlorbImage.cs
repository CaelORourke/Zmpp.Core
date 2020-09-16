/*
 * Created on 2006/05/09
 * Copyright (c) 2005-2010, Wei-ju Wu.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * Neither the name of Wei-ju Wu nor the names of its contributors may
 * be used to endorse or promote products derived from this software without
 * specific prior written permission.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

namespace Zmpp.Core.Blorb
{
    using System;
    using Zmpp.Core.Media;

    /// <summary>
    /// This class contains informations related to Blorb images and their scaling.
    /// </summary>
    /// <remarks>
    /// Scaling information is optional and probably only relevant
    /// to V6 games. BlorbImage also calculates the correct image size,
    /// according to the specification made in the Blorb standard specification.
    /// </remarks>
    public class BlorbImage : IZmppImage
    {
        /// <summary>
        /// Represents a rational number.
        /// </summary>
        public class Ratio
        {
            private readonly int numerator;
            private readonly int denominator;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="numerator">The numerator.</param>
            /// <param name="denominator">The denominator.</param>
            public Ratio(int numerator, int denominator)
            {
                this.numerator = numerator;
                this.denominator = denominator;
            }

            /// <summary>
            /// Gets the numerator.
            /// </summary>
            public int Numerator => numerator;

            /// <summary>
            /// Gets the denominator.
            /// </summary>
            public int Denominator => denominator;

            /// <summary>
            /// Gets the calculated value as a float value.
            /// </summary>
            public float Value => (float)numerator / denominator;

            /// <summary>
            /// Indicates whether this value specifies a valid value.
            /// </summary>
            /// <returns>true if the value is valid; otherwise false.</returns>
            public bool IsDefined => !(numerator == 0 && denominator == 0);

            public override string ToString() { return numerator + "/" + denominator; }
        }

        /// <summary>
        /// Represents resolution information.
        /// </summary>
        public class ResolutionInfo
        {
            private readonly Resolution standard;
            private readonly Resolution minimum;
            private readonly Resolution maximum;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="std">standard resolution.</param>
            /// <param name="min">minimum resolution.</param>
            /// <param name="max">maximum resolution.</param>
            public ResolutionInfo(Resolution std, Resolution min, Resolution max)
            {
                standard = std;
                minimum = min;
                maximum = max;
            }

            /// <summary>
            /// Gets the standard resolution.
            /// </summary>
            public Resolution Standard => standard;

            /// <summary>
            /// Gets the minimum resolution.
            /// </summary>
            public Resolution Minimum => minimum;

            /// <summary>
            /// Gets the maximum resolution.
            /// </summary>
            public Resolution Maximum => maximum;

            /// <summary>
            /// Calculates the ERF ("Elbow Room Factor").
            /// </summary>
            /// <param name="screenwidth">width of the display</param>
            /// <param name="screenheight">height of the display</param>
            /// <returns>elbow room factor</returns>
            public float ComputeERF(int screenwidth, int screenheight)
            {
                return Math.Min(screenwidth / standard.getWidth(), screenheight / standard.getHeight());
            }

            public override string ToString()
            {
                return "Std: " + standard.toString() + " Min: " + minimum.toString() +
                    " Max: " + maximum.toString();
            }
        }

        /// <summary>
        /// Represents scaling information.
        /// </summary>
        public class ScaleInfo
        {
            private readonly ResolutionInfo resolutionInfo;
            private readonly Ratio standard;
            private readonly Ratio minimum;
            private readonly Ratio maximum;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="resinfo">resolution information.</param>
            /// <param name="std">standard ratio.</param>
            /// <param name="min">minimum ratio.</param>
            /// <param name="max">maximum ratio.</param>
            public ScaleInfo(ResolutionInfo resinfo, Ratio std, Ratio min, Ratio max)
            {
                this.resolutionInfo = resinfo;
                this.standard = std;
                this.minimum = min;
                this.maximum = max;
            }

            /// <summary>
            /// Gets the resolution information.
            /// </summary>
            public ResolutionInfo ResolutionInfo => resolutionInfo;

            /// <summary>
            /// Gets the standard aspect ratio.
            /// </summary>
            public Ratio StdRatio => standard;

            /// <summary>
            /// Gets the minimum aspect ratio.
            /// </summary>
            public Ratio MinRatio => minimum;

            /// <summary>
            /// Gets the maximum aspect ratio.
            /// </summary>
            public Ratio MaxRatio => maximum;

            /// <summary>
            /// Computes the scaling ratio depending on the specified screen dimensions.
            /// </summary>
            /// <param name="screenwidth">width.</param>
            /// <param name="screenheight">height.</param>
            /// <returns>scaling ratio.</returns>
            public float ComputeScaleRatio(int screenwidth, int screenheight)
            {
                float value = resolutionInfo.ComputeERF(screenwidth, screenheight)
                  * standard.Value;

                if (minimum.IsDefined&& value < minimum.Value)
                {
                    value = minimum.Value;
                }
                if (maximum.IsDefined&& value > maximum.Value)
                {
                    value = maximum.Value;
                }
                return value;
            }

          public override string ToString()
            {
                return string.Format("std: {0}, min: {1}, max: {2}\n", standard.ToString(), minimum.ToString(), maximum.ToString());
            }
        }

        private readonly INativeImage image;
        private readonly Resolution resolution;
        private ScaleInfo scaleinfo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="image">NativeImage to wrap.</param>
        public BlorbImage(INativeImage image) { this.image = image; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="width">width.</param>
        /// <param name="height">height.</param>
        public BlorbImage(int width, int height)
        {
            resolution = new Resolution(width, height);
        }

        /// <summary>
        /// Gets the wrapped NativeImage.
        /// </summary>
        public INativeImage Image => image;

        /// <summary>
        /// Gets the scaling information.
        /// </summary>
        public ScaleInfo ScaleInfo1 => scaleinfo;

        /// <summary>
        /// Gets the size of the image scaled to the specified screen dimensions.
        /// </summary>
        /// <param name="screenwidth">screen width.</param>
        /// <param name="screenheight">screen height.</param>
        /// <returns>the scaled size.</returns>
        public Resolution GetSize(int screenwidth, int screenheight)
        {
            if (scaleinfo != null)
            {
                float ratio = scaleinfo.ComputeScaleRatio(screenwidth, screenheight);
                if (image != null)
                {
                    return new Resolution((int)(image.getWidth() * ratio),
                      (int)(image.getHeight() * ratio));

                }
                else
                {
                    return new Resolution((int)(resolution.getWidth() * ratio),
                        (int)(resolution.getHeight() * ratio));
                }
            }
            else
            {
                if (image != null)
                {
                    return new Resolution(image.getWidth(), image.getHeight());
                }
                else
                {
                    return new Resolution(resolution.getWidth(), resolution.getHeight());
                }
            }
        }

        /// <summary>
        /// Sets the ScaleInfo.
        /// </summary>
        /// <param name="aScaleinfo">ScaleInfo object.</param>
        public void SetScaleInfo(ScaleInfo aScaleinfo)
        {
            this.scaleinfo = aScaleinfo;
        }
    }
}
