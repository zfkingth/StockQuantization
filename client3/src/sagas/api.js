import { call, put } from 'redux-saga/effects'
import { unauthorizeUser } from '../actions/auth'


export function* callWith401Handle(...args) {
  if (process.env.REACT_APP_MOCK) return
  try {
    const data = yield call(...args)
    return data
  } catch (err) {
    console.info('request with error: ', err)
    if (err.status === 401) {
      yield put(unauthorizeUser())
    } else {
      throw err
    }
  }
}

export function* callWith401_403Handle(...args) {
  try {
    const data = yield call(...args);
    console.log('callWith401_403Handle data: ' + JSON.stringify(data));
    return data;
  } catch (ex) {

    const status = ex.status;
    if (status === 401) {
      ex.message = '未授权 由于凭据无效 访问被拒绝。';
    } else if (status === 403) {
      ex.message = '禁止访问 访问被拒绝。您无权使用所提供的凭据查看此目录或页面。';
    }
    console.log('callWith401_403Handle catch clause ' + JSON.stringify(ex));
    throw ex;
  }
}

