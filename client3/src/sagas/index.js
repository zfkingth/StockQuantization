import {
  takeLatest
} from 'redux-saga/effects'
import {
  select,
  take
} from 'redux-saga/effects'

import * as genericActions from '../actions/generic'
import * as genericSagas from './generic'

import * as authActions from '../actions/auth'
import * as authSagas from './auth'

import * as stockActions from '../actions/stock'
import * as stockSagas from './stock'

import * as manageActions from '../actions/manage'
import * as manageSagas from './manage'





export default function* saga() {
  const relations = [
    [genericActions, genericSagas],
    [authActions, authSagas],
    [stockActions, stockSagas],
    [manageActions, manageSagas],
  ]

  for (const [actions, sagas] of relations) {
    for (const [actionName, action] of Object.entries(actions)) {
      const saga = sagas[actionName]
      if (saga)
      {
        console.log( actionName+' is taken');
         yield takeLatest(action.getType(), saga)
      }
    }
  }
}

function* watchAndLog() {
  while (true) {
    const action = yield take('*')
    // if (action.type !== genericActions.moveMouse.getType() &&
    if (action.type !== genericActions.tick.getType()) {
      const state = yield select()


      console.log('action', action)

      console.log('state after', state)
    }
  }
}

export {
  watchAndLog
}