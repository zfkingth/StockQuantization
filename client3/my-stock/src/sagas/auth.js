import { put, call } from 'redux-saga/effects'
import { SubmissionError } from 'redux-form'

import { to } from '../actions/navigation'
import { receiveAuthData } from '../actions/auth'
import { LOGIN, REGISTER, UPDATE_USER, DELETE_USER } from '../constants/api'
import { post, patch, del } from '../utils/api'
import { startApp } from '../actions/generic'
import { callWith401_403Handle } from './api'

const authSaga = (url, thanGoTo) =>
  function* ({ payload: { values, reject } }) {
    try {
      const authData = yield post(url, values)
      yield put(receiveAuthData(authData))
      yield put(startApp())
      yield put(to(thanGoTo))
    } catch ({ status, message }) {
      yield call(reject, new SubmissionError(message))
    }
  }

export const submitLogin = authSaga(LOGIN, 'mainIndex');
export const submitRegister = authSaga(REGISTER, 'mainIndex');



export const updateUser =
  function* ({ payload: { id, values, reject } }) {
    try {
      yield callWith401_403Handle(patch, UPDATE_USER(id), values)
    } catch ({ status, message }) {
      yield call(reject, { status, message });
    }
  }


export const deleteUser =
  function* ({ payload: { id, reject } }) {
    try {
      //del 会合并路径和id
      yield callWith401_403Handle(del, DELETE_USER, id);
    } catch ({ status, message }) {
      yield call(reject, { status, message });
    }
  }
