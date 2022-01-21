import React, {useEffect} from 'react';
import axios from "axios";
import jwtDecode from 'jwt-decode';
import { useHistory, Link, withRouter } from 'react-router-dom';
import '../../Components/FormDropdown.css';
import OrderDropdown from './OrderDropdown';
import ReactGA from "react-ga";

ReactGA.initialize(process.env.GA_TRACKING_CODE);

const OrderHistory = (props) => {

    useEffect(() => {
        ReactGA.pageview(window.location.pathname + window.location.search);
    });

    // CSS
    return (
        <div>
            <OrderHistoryContent></OrderHistoryContent>
        </div>
    )

    // Return array of Order History Items
    function OrderHistoryContent() {

        // Loop for returning React Components
        let renderedItemArray = [];

        if (props.orderInfoList.length > 0) {
            for (let i = 0; i < props.orderInfoList.length; i++) {
                if (props.orderInfoList[i].order_status == "Delivered" || props.orderInfoList[i].order_status == "Cancelled") 
                {
                    renderedItemArray.push(<OrderDropdown key = {props.orderInfoList[i].order_id.toString()} title = {props.title} userInfo = {props.userInfo} orderInfoItem = {props.orderInfoList[i]} boxInfoItem = {props.boxInfoList[i]}></OrderDropdown>);
                } 
            }
        }

        /*const adminOrderArray = [];

        for (let i = 0; i < props.orderInfo.length; i++)
        {
            adminOrderArray.push({
                id: i+1,
                firstname: orderInfo[i].firstname,
                //...
            })
        }
        */
        
        // Output CSS
        if (renderedItemArray.length > 0) return renderedItemArray; 
        else return (<div> You have 0 previous orders... </div>);
        
    }

}



export default withRouter(OrderHistory);