
import { default as FormUpwardGap } from './components/stock/FormUpwardGap'
import { default as FormCloseBreakSearcher } from './components/stock/FormCloseBreakSearcher'
import { default as stockTemplate } from './components/stock'
import { default as FormUpMASearcher } from './components/stock/FormUpMASearcher'
import { default as FormCloseAppoach } from './components/stock/FormCloseApproachSearcher'
import { default as FormCiculatedMarket } from './components/stock/FormCirculatedMarketSearcher'
import { default as FormTurnOverRate } from './components/stock/FormTurnOverRate'
import { default as FormExceptZhangFu } from './components/stock/FormExceptZhangFu'
import { default as FormNRiseOpen } from './components/stock/FormNRiseOpen'
import { default as FormVolumeDecrease } from './components/stock/FormVolumeDecrease'
import { default as FormVolumeBreak } from './components/stock/FormVolumeBreak'
export { default as login } from './components/auth/login'
export { default as register } from './components/auth/register'
export { default as errorPage } from './components/ErrorPage'
export { default as manage } from './components/manage'
export { default as systemStatus } from './components/manage/SystemStatus'
export { default as systemInfo } from './components/About'
// default page
export { default as mainIndex } from './components/MyIndex'


const filterUpwardGap = () => { return stockTemplate(FormUpwardGap); };
export { filterUpwardGap };



const filterCloseBreak = () => { return stockTemplate(FormCloseBreakSearcher); };
export { filterCloseBreak  };


const filterUpMA = () => { return stockTemplate(FormUpMASearcher); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterUpMA };


const filterApproach = () => { return stockTemplate(FormCloseAppoach); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterApproach };




const filterCiculatedMarket = () => { return stockTemplate(FormCiculatedMarket); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterCiculatedMarket };



const filterTurnOverRate = () => { return stockTemplate(FormTurnOverRate); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterTurnOverRate };




const filterExceptZhangFu = () => { return stockTemplate(FormExceptZhangFu); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterExceptZhangFu };




const filterNRiseOpen = () => { return stockTemplate(FormNRiseOpen); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterNRiseOpen };




const filterVolumeDecrease = () => { return stockTemplate(FormVolumeDecrease); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterVolumeDecrease };


const filterVolumeBreak = () => { return stockTemplate(FormVolumeBreak); };

//这里输出的名称，state.navigation.page使用，
//要和调用to action里的参数匹配。
export { filterVolumeBreak };


