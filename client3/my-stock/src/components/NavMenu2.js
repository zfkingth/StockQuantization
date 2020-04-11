import './nav1.css';
import React from 'react';
import { unauthorizeUser } from '../actions/auth'
import { loadManage, loadSystemStatus, } from '../actions/manage'
import { connectTo } from '../utils/generic'

import { to } from '../actions/navigation'


const menufun = ({ to, unauthorizeUser, loadManage, loadSystemStatus, }) => {

    return (

        <div id="nav1">
            <div id="nav2">
                <ul>
                    <li><button>常用指标</button>
                        <ul >

                            <li><button onClick={() => to('IndexSZCZ')}> 深证成指</button></li>
                            <li><button onClick={() => to('IndexCyb')}> 创业板</button></li>
                            <li><button onClick={() => to('IndexHS300')}> 沪深300</button></li>

                        </ul></li>
                    <li><button>常规筛选</button>
                        <ul>
                            <li><button onClick={() => to('filterCloseBreak')}> 平台突破</button></li>
                            <li><button onClick={() => to('filterUpwardGap')}> 向上跳空</button></li>
                            <li><button onClick={() => to('filterUpMA')}>均线突破</button></li>
                            <li><button onClick={() => to('filterApproach')}>平台接近</button></li>
                            <li><button onClick={() => to('filterTurnOverRate')}> 换手率</button></li>
                            <li><button onClick={() => to('filterCiculatedMarket')}> 流通市值</button></li>

                        </ul></li>
                    <li><button>量价形态</button>
                        <ul>
                            <li><button onClick={() => to('filterVolumeBreak')}> 成交放量</button></li>
                            <li><button onClick={() => to('filterExceptZhangFu')}> 涨幅限定</button></li>
                            <li><button onClick={() => to('filterNRiseOpen')}>连续上涨</button></li>
                            <li><button onClick={() => to('filterVolumeDecrease')}>连续缩量</button></li>

                        </ul></li>

                    <li><button>帐户管理</button>
                        <ul>
                            <li><button onClick={() => loadSystemStatus()}>系统状态</button></li>
                            <li>
                                <button onClick={() => loadManage()}>用户管理</button>
                            </li>
                            <li><button onClick={() => to('systemInfo')}>关于系统</button></li>
                            <li><button onClick={() => unauthorizeUser()}>退出系统</button></li>

                        </ul></li>
                </ul>
            </div>
        </div>

    );
};

export default connectTo(
    null,
    { to, unauthorizeUser, loadManage, loadSystemStatus, },
    menufun
)