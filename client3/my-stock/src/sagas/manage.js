

import { get, post } from '../utils/api'
import * as URL from '../constants/api'
import { callWith401_403Handle } from './api';
import { put, call } from 'redux-saga/effects';
import { to, goToErrorPage } from '../actions/navigation'
import { receiveUserList, receiveSystemStatusData } from '../actions/manage'


const relations = [
  ['CalcLimitNum', URL.calcLimitNum],
  ['pullDaily', URL.pullDayData],
  ['pullF10', URL.pullF10],
  ['pullmargin', URL.pullMarginData],
  ['PullMarketDealData', URL.pullMarketDealData],
  ['pullRealTime', URL.pullRealTimeData],
  ['pullStockNames', URL.pullAllStockNames],
]


export const ariseSystemEventAction =
  function* ({ payload: eventName }) {
    try {

      let theUrl;
      for (const [name, url] of relations) {
        if (name === eventName) {
          theUrl = url;
          break;
        }
      }

      console.log("arise fucntion is called")

      yield callWith401_403Handle(post, theUrl);
    } catch (err) {

      yield put(goToErrorPage(err));
    }
  }

export const clearDateAction =
  function* ({ payload: eventName }) {
    try {

      yield callWith401_403Handle(post, URL.CLEARDATE, eventName);
    } catch (err) {

      yield put(goToErrorPage(err));
    }
  }




export const loadManage =
  function* () {
    try {
      console.log('enter load manage try clause');
      const data = yield callWith401_403Handle(post, URL.GETUSERS);
      yield put(receiveUserList(data));
      yield put(to('manage'));
    } catch (err) {
      console.log('enter load manage catch  clause');
      yield put(goToErrorPage(err));
    }
    console.log('exit load manage   clause');
  }


export const loadSystemStatus =
  function* () {
    try {
      console.log('enter loadSystemStatus try clause');
      const [num, statusTable] = yield [
        yield callWith401_403Handle(get, URL.GETTASKNUM),
        yield callWith401_403Handle(get, URL.GETSTATUSTABLE)
      ]

      yield put(receiveSystemStatusData({ num, statusTable }));
      yield put(to('systemStatus'));
    } catch (err) {
      console.log('enter loadSystemStatus catch  clause');
      yield put(goToErrorPage(err));
    }
    console.log('exit loadSystemStatus clause');
  }

export const pullRealTimeAction =
  function* ({ payload: { values, resolve, reject } }) {
    try {
      yield callWith401_403Handle(post, URL.pullRealTimeData);
      yield call(resolve);
    } catch (err) {

      yield call(reject, err);
    }
  }




// export const pulBasicDataAction =
//   function* ({ payload: { values, resolve, reject } }) {
//     try {
//       yield callWith401_403Handle(post, URL.PULBASICDATA, values);
//       yield call(resolve);
//     } catch (err) {

//       yield call(reject, err);
//     }
//   }


