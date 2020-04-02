
import React from "react";

import styled from 'styled-components'


import { get, fetchData } from '../../utils/api'
import * as URL from '../../constants/api'
import CircularProgress from '@material-ui/core/CircularProgress';

import { transformError } from '../../utils/api'


const MulRowsWrap = styled.div`
 
  flex-direction:column;
  display: flex;
  justify-content: flex-start;
  align-items: center;
`

const Loading = styled.div`
 
  display: flex;
  justify-content: center;
  align-items: center;
`
const StyleError = styled.p`
 
color:red;
`


export default class tempcontrol extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = { receivedData: undefined, error: undefined };
  }

  componentDidMount() {
    this.fetchData();


  }


  fetchData = async function () {
    try {
      const dateInfo = await fetchData(get, URL.GETDATENFO);
      console.log("received data is : " + dateInfo);

      this.setState({ receivedData: dateInfo });
    } catch (err) {

      console.log("received data error : " + err);
      this.setState({ error: transformError(err) });
    }

  }


  render() {
    return this.state.receivedData ? (
      <MulRowsWrap>
        <p>服务器中最新的分红除权数据时间为：{this.state.receivedData.gongGao.replace('T', ' ')}</p>
        <p>服务器中最新的历史数据时间为：        {this.state.receivedData.daily.replace('T', ' ')}</p>
        <p>服务器中最新的实时数据时间为：        {this.state.receivedData.realTime.replace('T', ' ')}</p>

      </MulRowsWrap>
    ) : this.state.error ?
        (
          <StyleError>错误：{this.state.error}</StyleError>
        )
        : (
          <Loading>
            <CircularProgress />
          </Loading>
        )

  }
}


