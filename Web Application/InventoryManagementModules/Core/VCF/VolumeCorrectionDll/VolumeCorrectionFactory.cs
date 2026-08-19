
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCF
{
    public class VolumeCorrectionFactory
    {
        public static TankBaseVcf GetVolumeCorrection(Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor bVolCorecTypeMajor, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor bVolCorecTypeMinor, bool bUseDensity, bool doHydroCorrection , bool frenchWM, bool japanWM, bool forcetoFourDigits)
        {
            TankBaseVcf VolCorrRet = null;
            switch (bVolCorecTypeMajor)
            {
                //*****************************************************************************
                // CORR_NONE
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_NONE:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_NONE:
                                {
                                    VolCorrRet = null;// new TankVcfNone();
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        }
                        break;

                    } // End case CORR_NONE

                //*****************************************************************************
                // CORR_60_F
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi6A();                                       
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi5a();
                                    }
                                    break;
                                } // End case CORR_API6A

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6B:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi6b();                                      
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi5b();                                        
                                    }
                                    break;
                                } // End case CORR_API6B

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6C:
                                {
                                    VolCorrRet = new TankApi6c();  
                                    break;
                                } // End case CORR_API6C

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6D:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi6d();  
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi5d();  
                                    }
                                    break;
                                } // End case CORR_API6D

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API24E:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi24e();  
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi23e();  
                                    }
                                    break;
                                } // End case CORR_API24E

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case CORR_60_F

                //*****************************************************************************
                // CORR_15_C
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54a();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53a();
                                    }
                                    break;

                                } // End case CORR_API54A

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54b();                                       
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53b();
                                     }
                                    break;

                                } // End case CORR_API54B

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C:
                                {
                                    VolCorrRet = new TankApi54c();
                                    break;

                                } // End case CORR_API54C

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54d();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53d();
                                    }
                                    break;
                                } // End case CORR_API54D

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60A:
                                {
                                    VolCorrRet = new TankApi60a();
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60B:
                                {
                                    VolCorrRet = new TankApi60b();
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60D:
                                {
                                    VolCorrRet = new TankApi60d();
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54a_30C();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53a_30C();
                                    }
                                    break;

                                } // End case CORR_API54A_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54b_30C();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53b_30C();
                                    }
                                    break;

                                } // End case CORR_API54B_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C_30:
                                {
									VolCorrRet = new TankApi54c_30C();
                                    break;

                                } // End case CORR_API54C_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54d_30C();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53d_30C();
                                    }
                                    break;

                                } // End case CORR_API54D_30

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case CORR_API_C

                //*****************************************************************************
                // CORR_POLYNOMIAL_F
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_POLYNOMIAL_F:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_POLYNOMIAL:
                                {
                                    VolCorrRet = new TankVcfPolynomial();
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)

                        break;

                    } // End case POLYNOMIAL

                //*****************************************************************************
                // CORR_LPG_C
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_LPG_C:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_LPG:
                                {
                                    VolCorrRet = new TankVcfLpg();
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case LPG

                //*****************************************************************************
                // D1555_HYDRO_60F
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_F_2004:
                    {
						// 2004 and 1980 are the same so just call the 1980 objects.
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE:
                                {
									VolCorrRet = new TankVcfD1555_60F_Benzene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_Toluene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_M_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_MXylene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_STYRENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_Styrene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_O_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_OXylene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_P_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_PXylene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_CycloHexane_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_EthylBenzene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CUMENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_Cumene_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_300Aromatic_1980();
									break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_350_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_350Aromatic_1980();
									break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case D1555_2004

                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_Benzene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_Toluene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_M_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_MXylene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_STYRENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_Styrene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_O_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_OXylene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_P_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_PXylene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_CycloHexane();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_EthylBenzene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CUMENE:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_Cumene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_300Aromatic();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_350_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_2009_60F_350Aromatic();
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case D1555_2009
                //*****************************************************************************
                // CORR_ASTM_D1555_C_2004
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_C_2004:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE:
                                {
									VolCorrRet = new TankVcfD1555_15C_Benzene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_Toluene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_M_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_MXylene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_STYRENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_Styrene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_O_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_OXylene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_P_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_PXylene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_CycloHexane();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_EthylBenzene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CUMENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_Cumene();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_300Aromatic();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_350_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_350Aromatic();
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End of switch(bVolCorecTypeMinor)

                        break;

                    } // End case D1555_HYDRO_15C:

                //*****************************************************************************
                // CORR_NONE 1980
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_NONE_1980:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_NONE:
                                {
                                    VolCorrRet = null; // new TankVcfNone_1980);
                                    
                                        

                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        }
                        break;

                    } // End case CORR_NONE

                //*****************************************************************************
                // CORR_API_F_1980
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F_1980:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi6a_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi5a_1980();
                                    }
                                    break;
                                } // End case CORR_API6A

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6B:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi6b_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi5b_1980();
                                    }

                                    break;
                                } // End case CORR_API6B

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6C:
                                {
                                    VolCorrRet = new TankApi6c_1980();
                                    break;
                                } // End case CORR_API6C

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6D:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi6d_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi5d_1980();
                                    }
                                    break;

                                } // End case CORR_API6D

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case CORR_60_F_1980

                //*****************************************************************************
                // CORR_API_C_1980
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54a_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53a_1980();
                                    }
                                    break;

                                } // End case CORR_API54A

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54b_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53b_1980();
                                    }
                                    break;

                                } // End case CORR_API54B

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C:
                                {
                                    VolCorrRet = new TankApi54c_1980();
                                    break;

                                } // End case CORR_API54C

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54d_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53d_1980();
                                    }
                                    break;
                                } // End case CORR_API54D

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60A:
                                {
                                    VolCorrRet = new TankApi60a_1980();
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60B:
                                {
                                    VolCorrRet = new TankApi60b_1980();
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60D:
                                {
                                    VolCorrRet = new TankApi60d_1980();
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54a_30C_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53a_30C_1980();
                                    }
                                    break;

                                } // End case CORR_API54A_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54b_30C_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53b_30C_1980();
                                    }
                                    break;

                                } // End case CORR_API54B_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C_30:
                                {
                                    VolCorrRet = new TankApi54c_30C_1980();
                                    break;

                                } // End case CORR_API54C_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = new TankApi54d_30C_1980();
                                    }
                                    else
                                    {
                                        VolCorrRet = new TankApi53d_30C_1980();
                                    }
                                    break;

                                } // End case CORR_API54D_30

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case CORR_15_C_1980

                //*****************************************************************************
                // POLYNOMIAL_1980
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_POLYNOMIAL_F_1980:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_POLYNOMIAL:
                                {
                                    VolCorrRet = new TankVcfPolynomial_1980();
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)

                        break;

                    } // End case POLYNOMIAL_1980

                //*****************************************************************************
                // CORR_LPG_C_1980
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_LPG_C_1980:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_LPG:
                                {
                                    VolCorrRet = new TankVcfLpg_1980();
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case LPG_1980

                //*****************************************************************************
                // CORR_ASTM_D1555_F_1980
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_F_1980:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_Benzene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE:
                                {
                                    VolCorrRet =  new TankVcfD1555_60F_Toluene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_M_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_MXylene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_STYRENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_Styrene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_O_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_OXylene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_P_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_PXylene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_CycloHexane_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_EthylBenzene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CUMENE:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_Cumene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_300Aromatic_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_350_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_60F_350Aromatic_1980();
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case D1555_HYDRO_60F_1980

                //*****************************************************************************
                // CORR_ASTM_D1555_C_1980
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_C_1980:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_Benzene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_Toluene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_M_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_MXylene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_STYRENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_Styrene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_O_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_OXylene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_P_XYLENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_PXylene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_CycloHexane_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_EthylBenzene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CUMENE:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_Cumene_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_300Aromatic_1980();
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_350_AROMATIC:
                                {
                                    VolCorrRet = new TankVcfD1555_15C_350Aromatic_1980();
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End of switch(bVolCorecTypeMinor)

                        break;

                    } // End case D1555_HYDRO_15C_1980

                //*****************************************************************************
                // CORR_JAPAN_NONE
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_NONE:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_NONE:
                                {
                                    VolCorrRet = null; // new TankJapanNone);
                                    
                                        

                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;
                        }
                        break;

                    } // End case CORR_NONE

                //*****************************************************************************
                // CORR_SAKURA_JIS_2249
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_JIS_2249:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54a);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53a);
                                        
                                            
                                    }
                                    break;

                                } // End case CORR_API54A

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54b);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53b);
                                        
                                            
                                    }
                                    break;

                                } // End case CORR_API54B

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C:
                                {
                                    VolCorrRet = null; // new TankJapan54c);
                                    
                                        
                                    break;

                                } // End case CORR_API54C

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54d);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53d);
                                        
                                            
                                    }
                                    break;
                                } // End case CORR_API54D

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54a_30C);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53a_30C);
                                        
                                            
                                    }
                                    break;

                                } // End case CORR_API54A_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54b_30C);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53b_30C);
                                        
                                            
                                    }
                                    break;

                                } // End case CORR_API54B_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C_30:
                                {
                                    VolCorrRet = null; // new TankJapan54c_30C);
                                    
                                        
                                    break;

                                } // End case CORR_API54C_30

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54d_30C);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53d_30C);
                                        
                                            
                                    }
                                    break;

                                } // End case CORR_API54D_30

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case CORR_Japan_JIS_2249

                //*****************************************************************************
                // CORR_JAPAN_JIS_2250
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_JIS_2250:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_JIS_TABLE2:
                                {
                                    VolCorrRet = null; // new TankJapanTable2);
                                    
                                        
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case CORR_Japan_JIS_2250

                //*****************************************************************************
                // CORR_JAPAN_ASTM_D1250
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1250:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ASTM_TABLE55:
                                {
                                    VolCorrRet = null; // new TankJapanTable55);
                                    
                                        
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54A:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapanTable6X_54A);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapanTable6X_53A);
                                        
                                            
                                    }
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ASTM_TABLE6X_54B:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapanTable6X_54B);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapanTable6X_53B);
                                        
                                            
                                    }
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ASTM_TABLE2:
                                {
                                    VolCorrRet = null; // new TankJapanAstmTable2);
                                    
                                        
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case TankBaseVcf.CORR_Japan_ASTM_D1250

                //*****************************************************************************
                // TankBaseVcf.CORR_Japan_CHEMICAL
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_CHEMICAL:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_JIS_CHEMICAL1:
                                {
                                    VolCorrRet = null; // new TankJapanChemical1);
                                    
                                        
                                    break;
                                }

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_JIS_CHEMICAL2:
                                {
                                    VolCorrRet = null; // new TankJapanChemical2);
                                    
                                        
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case TankBaseVcf.CORR_Japan_D1250

                //*****************************************************************************
                // TankBaseVcf.CORR_JAPAN_ASTM_D1555
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1555:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_BENZENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_Benzene);                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TOLUENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_Toluene);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_M_XYLENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_MXylene);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_STYRENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_Styrene);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_O_XYLENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_OXylene);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_P_XYLENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_PXylene);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CYCLO_HEXANE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_CycloHexane);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ETHYL_BENZENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_EthylBenzene);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CUMENE:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_Cumene);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_300_AROMATIC:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_300Aromatic);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_350_AROMATIC:
                                {
                                    VolCorrRet = null; // new TankJapan_D1555_350Aromatic);
                                    
                                        
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End of switch(bVolCorecTypeMinor)

                        break;

                    } // End case TankBaseVcf.CORR_JAPAN_D1555

                //*****************************************************************************
                // TankBaseVcf.CORR_SAKURA_JIS_2249_Table
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_TABLE:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54aTable);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53aTable);
                                        
                                            
                                    }
                                    break;

                                } // End case TankBaseVcf.CORR_API54A

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_TABLE:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54bTable);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53bTable);
                                        
                                            
                                    }
                                    break;

                                } // End case TankBaseVcf.CORR_API54B

                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_TABLE:
                                {
                                    if (!bUseDensity)
                                    {
                                        VolCorrRet = null; // new TankJapan54dTable);
                                        
                                            
                                    }
                                    else
                                    {
                                        VolCorrRet = null; // new TankJapan53dTable);
                                        
                                            
                                    }
                                    break;
                                } // End case TankBaseVcf.CORR_API54D

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case TankBaseVcf.CORR_Japan_JIS_2249_Table
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_GBT:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_APIGBT60A:
                                {
                                    VolCorrRet = null; // new TankApiGBT60a);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_APIGBT60B:
                                {
                                    VolCorrRet = null; // new TankApiGBT60b);
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_APIGBT60D:
                                {
                                    VolCorrRet = null; // new TankApiGBT60d);
                                    
                                        
                                    break;
                                }
                        }
                        break;
                    }
                //*****************************************************************************
                // TankBaseVcf.CORR_GOST
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_GOST:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_3900_85_20C:
                                {
                                    VolCorrRet = new TankVcfGOST_3900_85_20C();
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End of switch(bVolCorecTypeMinor)

                        break;

                    } // End case TankBaseVcf.CORR_GOST:

                //*****************************************************************************
                // TankBaseVcf.CORR_ASPHALT
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASPHALT:		// Added (BDS 24-Aug-2004)
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D4311DEGC_2004:
                                {
                                    VolCorrRet = new TankVcfAsphaltD4311();
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D4311DEGF_2004:
                                {
                                    VolCorrRet = new TankVcfAsphaltD4311DegF();
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_TABLE7:
                                {
                                    VolCorrRet = new TankVcfAsphaltTable7();
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D4311DEGC_2009:
                                {
                                    VolCorrRet = new TankVcfAsphaltD4311DegC_2009();
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D4311DEGF_2009:
                                {
                                    VolCorrRet = new TankVcfAsphaltD4311DegF_2009();
                                    
                                        
                                    break;
                                }
                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End of switch(bVolCorecTypeMinor)

                        break;

                    } // End case TankBaseVcf.CORR_ASPHALT:


                //*****************************************************************************
                // TankBaseVcf.CORR_ASTM_D1250_1952
                //*****************************************************************************
                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1250_1952:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_D125020DEGC:
                                {
                                    VolCorrRet = new TankVcfD1250Lpg();
                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case TankBaseVcf.CORR_ASTM_D1250_1952

                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004:
                    {
                        switch (bVolCorecTypeMinor)
                        {
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_ALPHA60_SUPPLIED:
                                {
                                    VolCorrRet = new TankVcfAPI2004Alpha60();
                                    
                                        
                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CRUDE_OIL:
                                {
                                    VolCorrRet = new TankVcfAPI2004CrudeOil();


                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_REFINED_PRODUCTS:
                                {
                                    VolCorrRet = new TankVcfAPI2004RefProducts();


                                    break;
                                }
                            case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_LUBRICATION_OIL:
                                {
                                    VolCorrRet = new TankVcfAPI2004LubeOils();


                                    break;
                                }

                            // Minor Correction type not found
                            default:
                                {
                                    VolCorrRet = null;
                                }

                                break;

                        } // End switch(bVolCorecTypeMinor)
                        break;

                    } // End case TankBaseVcf.CORR_ASTM_D1250_1952
                //*****************************************************************************
                // Else the Major correction type can not be found
                //*****************************************************************************
                default:
                    {
                        VolCorrRet = null;
                    }

                    break;

            } // End of switch(bVolCorecTypeMajor)
            if(VolCorrRet!= null)
            {
                if(ApiUnit.ApiUnit_initalize() != Error.NO_ERROR)
                {
                    throw new Exception("ApiUnit Initialization Failed");
                }

                if (ApiOilProduct.Initalize() != Error.NO_ERROR)
                {
                    throw new Exception("ApiOilProduct Initialization Failed");
                }

                VolCorrRet.DoHydroCorrection = doHydroCorrection;
                VolCorrRet.FrenchWM = frenchWM;
                VolCorrRet.JapanWM = japanWM;
                VolCorrRet.ForcetoFourDigits = forcetoFourDigits;
            }
            return VolCorrRet;
        }
    }
}
