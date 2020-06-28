import { put, call, } from 'redux-saga/effects'


import * as a from '../actions/stock'
import * as URL from '../constants/api'
import { post } from '../utils/api'

const searchSaga = (url) =>
  function* ({ payload: { values, resolve, reject } }) {
    try {
      yield put(a.stockRequestStart());
      const data = yield post(url, values);
      // yield put(receiveStockSearchResult(data));
      console.log('searchSaga data: ' + data);
      yield call(resolve, data);
    } catch (ex) {
      const { message } = ex;

      console.log('searchSaga catch clause');
      yield call(reject, new Error(message));
    } finally {
      console.log('searchSaga finally clause')
      yield put(a.stockRequestEnd());
    }
  }


// const searchSaga = (url) =>
// function* ({ payload: { values,resolve, reject } }) {
//   try {
//     yield put(stockTaskStart());
//     const data = yield call(post, url, values);
//     yield put(receiveStockSearchResult(data));
//     yield call(resolve, data);
//   } catch ({ status, message }) {
//     console.log('reject function: '+reject);
//     yield call(reject, new Error(message));
//   }finally
//   {
//     yield put(stockTaskEnd());
//   }
// }

//输出的这个名称要和action的名称匹配
export const searchUpwardGap = searchSaga(URL.SEARCHUPWARDGAP)
export const searchColseBreak = searchSaga(URL.SEARCHCOLSEBREAK)
export const searchUpMA = searchSaga(URL.UPMA)
export const searchUpMATwice = searchSaga(URL.UPMATwice)
export const searchApproach = searchSaga(URL.APPROACH)
export const searchCirculatedMarket = searchSaga(URL.CIRCULATEDMARKET)
export const searchTurnOverRate = searchSaga(URL.TURNOVERRATE)
export const searchExceptZhangFu = searchSaga(URL.EXCEPTZHANGFU)
export const searchNRiseOpen = searchSaga(URL.NRISEOPEN)
export const searchVolumeDecrease = searchSaga(URL.VOLUMEDECREASE)
export const searchVolumeBreak = searchSaga(URL.VOLUMEBREAK)
export const searchMACD = searchSaga(URL.SEARCHMACD)
