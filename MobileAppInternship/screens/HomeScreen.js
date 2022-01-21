import React, { Component } from 'react';
import { Platform, TouchableOpacity, Text, TextInput, StyleSheet, View, Modal, Image, ScrollView, TouchableWithoutFeedback, Dimensions } from 'react-native';
import { Container, Header, Left, Right, Body, Title, Content, Footer, FooterTab, Icon, Button} from 'native-base';
import AsyncStorage from '@react-native-community/async-storage';
import Icon1 from 'react-native-vector-icons/FontAwesome5';
import Icon2 from 'react-native-vector-icons/MaterialCommunityIcons';
import styles from '../styles/HomeScreenStyleSheet';
import stylesLandscape from '../styles/HomeScreenLandscape';

export default class HomeScreen extends Component {
  constructor(props) {
    super(props);
    //States for notification modal, side menu, username, and color inputs
    this.state= {
      screen: Dimensions.get('window'),
      modal: 'false',
      modalColor: "#66bb77",
      notificationColor: "#66bb77",
      menu: 'false',
      username: '',
      thumbsUp: 'white',
      mood1Color: "#888888",
      mood2Color: "#888888",
      mood3Color: "#888888",
      mood4Color: "#888888",
      mood5Color: "#888888",
    }
  }
  //Set username from local storage
  setUsername = async () => {
    const userData = await AsyncStorage.getItem('userData');
    const parseData = JSON.parse(userData);
    const user = parseData.username;
    this.setState({username: user});
  }
  //Clear local storage when signing out
  clearDataToStorage = (token, userId, username) => {
    AsyncStorage.setItem('userData', JSON.stringify({token: token, userId: userId, username: username}));
    AsyncStorage.setItem('score', JSON.stringify({correct: token, incorrect: token, percentage: token}));
  };
  //Set username once after components render
  componentDidMount() {
    this.setUsername();
  }
  //Get assessment score color based from current day and array of results
  getDateColor(day){
    var now = new Date();
    //Example array of results
    var scoreColor = ['#F6B900','#F64000','#00CD98','#00CD98','#00CD98','#00CD98', '#00CD98'];
    var today = now.getDay();
    if (today == day){
      return "white";
    }
    else if(day < today){
      return scoreColor[day];
    }
    else{
      return "white";
    }
  }
  //Get assesment text score color based from current day
  getDateText(day){
    var now = new Date();
    var today = now.getDay();
    if (today > day){
      return "white";
    }
    else{
      return "black";
    }
  }
  //Screen orientation logic
  getOrientation(){
    if (this.state.screen.width > this.state.screen.height) {
      return 'LANDSCAPE';
    }
    else {
      return 'PORTRAIT';
    }
  }
  getStyle(){
    if (this.getOrientation() === 'LANDSCAPE') {
      return stylesLandscape;
    }
    else {
      return styles;
    }
  }
  onLayout(){
    this.setState({screen: Dimensions.get('window')});
  }
  //UI for HomeScreen
  render() {
    return (
      <Container style={styles.container}>
        {/*Notification modal*/}
        <Modal
          animationType="fade"
          transparent={true}
          visible={this.state.modal}
          supportedOrientations={['portrait', 'landscape']}
        >
          <TouchableOpacity
            style={styles.modalContainer}
            activeOpacity={1}
            onPressOut={() => {this.setState({modal: false, notificationColor: "#555555"})}}
          >
              <TouchableWithoutFeedback>
                <View style={styles.modal}>
                  <View style={styles.modalArrow}>
                    <Icon2 name="triangle" size= {30} color= {"white"} style= {{paddingTop: 15}}/>
                  </View>
                  <View style={styles.modalContent}>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Welcome Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="user" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                  </View>
                </View>
              </TouchableWithoutFeedback>
          </TouchableOpacity>
        </Modal>
        {/*Side Menu modal*/}
        <Modal
          animationType="slide"
          transparent={true}
          visible={this.state.menu}
          supportedOrientations={['portrait', 'landscape']}
        >
          <View style={styles.menuContainer}>
            <View style={styles.menu}>
              {/*Close menu button*/}
              <View style={styles.menuClose}>
                <Icon2 name="close" size= {40} color= {"black"} onPress= {() => this.setState({menu: false})} style={{paddingRight: 10, paddingBottom: 5}}/>
              </View>
              {/*Menu profile*/}
              <View style={styles.menuProfile}>
                <View style={styles.menuProfileImage}>
                  <Icon1 name="user" size= {100} color= {"black"}/>
                </View>
                <View style={styles.menuProfileName}>
                  <Text style={styles.textMenuProfile}> {this.state.username} </Text>
                  {/*Sign out button*/}
                  <TouchableOpacity
                    onPress={() => {
                      this.clearDataToStorage(null, null, null);
                      this.props.navigation.navigate({routeName: 'Login'});
                      this.setState({menu: false});
                    }}
                  >
                    <Text style={styles.textMenuSignOut}> Sign Out </Text>
                  </TouchableOpacity>
                </View>
              </View>
              <View style={styles.menuContent}>
                {/*Analysis*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Analysis </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*Settings*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Settings </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*Help*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Help </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*What We Do*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> What We Do </Text>
                    </View>
                  </TouchableOpacity>
                </View>
              </View>
            </View>
          </View>
        </Modal>
        {/*Header*/}
        <Header style={{backgroundColor: 'white', borderBottomWidth: 0}}>
          <Left>
              <Button transparent>
                <Icon1 name='align-left' size= {25} color= {"black"} onPress={() => {
                  this.setState({menu: true});
                  this.setUsername();
                }}/>
              </Button>
            </Left>
            <Body>
              <Image source= {require('C:/Users/dadir/AppSignUp/images/cognifyx_infinitum_logo.png')}/>
            </Body>
            <Right>
              <Button transparent>
                <Icon1 name='bell' size= {25} color= {this.state.modalColor} solid= {true} onPress={() => {
                  this.setState({modal: true, modalColor: "black"})
                }}/>
              </Button>
            </Right>
        </Header>
        <ScrollView>
          <View style= {this.getStyle().content} onLayout = {this.onLayout.bind(this)}>
            {/*Take Assessment module*/}
            <View style= {this.getStyle().module1} onLayout = {this.onLayout.bind(this)}>
              <View>
              {/*Assessment results graphic*/}
              <View style= {styles.graphic}>
               <View style= {styles.chart}>
                {/*Sunday*/}
                 <View style= {styles.chartItem}>
                   <View style= {[this.getStyle().result, {backgroundColor: this.getDateColor(0)}]} onLayout = {this.onLayout.bind(this)}>
                     <View style= {styles.weekday}>
                       <Text style={[styles.textAssessment, {color: this.getDateText(0)}]}> S </Text>
                     </View>
                   </View>
                 </View>
                 <View style={styles.connector}>
                 </View>
                 {/*Monday*/}
                  <View style= {styles.chartItem}>
                    <View style= {[this.getStyle().result, {backgroundColor: this.getDateColor(1)}]} onLayout = {this.onLayout.bind(this)}>
                      <View style= {styles.weekday}>
                        <Text style={[styles.textAssessment, {color: this.getDateText(1)}]}> M </Text>
                      </View>
                    </View>
                  </View>
                  <View style={styles.connector}>
                  </View>
                  {/*Tuesday*/}
                  <View style= {styles.chartItem}>
                    <View style= {[this.getStyle().result, {backgroundColor: this.getDateColor(2)}]} onLayout = {this.onLayout.bind(this)}>
                      <View style= {styles.weekday}>
                        <Text style={[styles.textAssessment, {color: this.getDateText(2)}]}> T </Text>
                      </View>
                    </View>
                  </View>
                  <View style={styles.connector}>
                  </View>
                  {/*Wednesday*/}
                  <View style= {styles.chartItem}>
                    <View style= {[this.getStyle().result, {backgroundColor: this.getDateColor(3)}]} onLayout = {this.onLayout.bind(this)}>
                      <View style= {styles.weekday}>
                        <Text style={[styles.textAssessment, {color: this.getDateText(3)}]}> W </Text>
                      </View>
                    </View>
                  </View>
                  <View style={styles.connector}>
                  </View>
                  {/*Thursday*/}
                  <View style= {styles.chartItem}>
                    <View style= {[this.getStyle().result, {backgroundColor: this.getDateColor(4)}]} onLayout = {this.onLayout.bind(this)}>
                      <View style= {styles.weekday}>
                        <Text style={[styles.textAssessment, {color: this.getDateText(4)}]}> T </Text>
                      </View>
                    </View>
                  </View>
                  <View style={styles.connector}>
                  </View>
                  {/*Friday*/}
                  <View style= {styles.chartItem}>
                    <View style= {[this.getStyle().result, {backgroundColor: this.getDateColor(5)}]} onLayout = {this.onLayout.bind(this)}>
                      <View style= {styles.weekday}>
                        <Text style={[styles.textAssessment, {color: this.getDateText(5)}]}> F </Text>
                      </View>
                    </View>
                  </View>
                  <View style={styles.connector}>
                  </View>
                  {/*Saturday*/}
                  <View style= {styles.chartItem}>
                    <View style= {[this.getStyle().result, {backgroundColor: this.getDateColor(6)}]} onLayout = {this.onLayout.bind(this)}>
                      <View style= {styles.weekday}>
                        <Text style={[styles.textAssessment, {color: this.getDateText(6)}]}> S </Text>
                      </View>
                    </View>
                  </View>
                </View>
              </View>
              {/*Take Assessment button*/}
              <View style= {this.getStyle().assessmentButton} onLayout = {this.onLayout.bind(this)}>
                <TouchableOpacity
                  style= {this.getStyle().assessmentButton} onLayout = {this.onLayout.bind(this)}
                  onPress={() => this.props.navigation.navigate({routeName: 'Assessment'})}
                >
                  <View style= {styles.assessmentIcon}>
                    <Icon1 name="clipboard" size= {30} color= {"white"}/>
                  </View>
                  <Text style= {styles.textAssessment}> Take Assessment </Text>
                </TouchableOpacity>
              </View>
              </View>
            </View>
            {/*Brain Health Tip module*/}
            <View style= {this.getStyle().module2} onLayout = {this.onLayout.bind(this)}>
              <TouchableOpacity>
              <View style= {styles.brainhealthTip}>
               <View style= {styles.tipHeader}>
                 <View style= {styles.brainhealthIcon}>
                   <Icon1 name="bullseye" size= {30} color= {"white"}/>
                 </View>
                 <Text style= {styles.textTipHeader}> Today's Brain Health Tip </Text>
               </View>
               <View style= {styles.tipTitle}>
                <Text style= {styles.textTipTitle} numberOfLines= {2}>What you eat has an impact on your mood! </Text>
               </View>
               <View style= {styles.tipBody}>
                <Text style= {styles.textTipBody} numberOfLines= {4}>Do not skip meals, eat lightly every 3 to 4 hours to keep glucose levels stable. Avoid excess white flour and sugar and load up on fruits and vegitables.</Text>
               </View>
               <View style= {styles.tipFooter}>
                <Text style= {styles.textTipFooter}> Was it helpful?   </Text>
                {/*Thumbs Up button*/}
                <TouchableOpacity
                  onPress={() => {
                    if(this.state.thumbsUp == "white")
                    {
                      this.setState({thumbsUp: "#22bb33"})
                    }
                    else
                    {
                      this.setState({thumbsUp: "white"})
                    }
                  }}
                >
                  <Icon1 name="thumbs-up" size= {20} color= {this.state.thumbsUp}/>
                </TouchableOpacity>
               </View>
              </View>
              </TouchableOpacity>
            </View>
            {/*Mood Diary module*/}
            <View style= {this.getStyle().module3} onLayout = {this.onLayout.bind(this)}>
              <View style= {styles.moodDiary}>
               <View style= {styles.moodHeader}>
                <Text style= {styles.textMood}> How are you feeling today? </Text>
               </View>
               <View style= {styles.moodBody}>
                 {/*Joyful*/}
                 <View style= {styles.moodItem}>
                  <TouchableOpacity
                    onPress={() => {
                      if(this.state.mood1Color == "#888888")
                      {
                        this.setState({mood1Color: "#F6B90088"})
                      }
                      else
                      {
                        this.setState({mood1Color: "#888888"})
                      }
                    }}
                  >
                   <View style= {styles.moodIcon}>
                    <Icon1 name= "grin-beam" size= {55} color= {this.state.mood1Color}/>
                   </View>
                  </TouchableOpacity>
                   <View style= {styles.moodFeeling}>
                    <Text style= {styles.textMood}> Joyful </Text>
                   </View>
                 </View>
                 {/*Fine*/}
                 <View style= {styles.moodItem}>
                 <TouchableOpacity
                   onPress={() => {
                     if(this.state.mood2Color == "#888888")
                     {
                       this.setState({mood2Color: "#FE900F88"})
                     }
                     else
                     {
                       this.setState({mood2Color: "#888888"})
                     }
                   }}
                 >
                   <View style= {styles.moodIcon}>
                    <Icon1 name= "smile" size= {55} color= {this.state.mood2Color}/>
                   </View>
                  </TouchableOpacity>
                   <View style= {styles.moodFeeling}>
                    <Text style= {styles.textMood}> Fine </Text>
                   </View>
                 </View>
                 {/*Meh*/}
                 <View style= {styles.moodItem}>
                 <TouchableOpacity
                   onPress={() => {
                     if(this.state.mood3Color == "#888888")
                     {
                       this.setState({mood3Color: "#FE650F88"})
                     }
                     else
                     {
                       this.setState({mood3Color: "#888888"})
                     }
                   }}
                 >
                   <View style= {styles.moodIcon}>
                    <Icon1 name= "meh" size= {55} color= {this.state.mood3Color}/>
                   </View>
                </TouchableOpacity>
                   <View style= {styles.moodFeeling}>
                    <Text style= {styles.textMood}> Meh </Text>
                   </View>
                 </View>
                 {/*Sad*/}
                 <View style= {styles.moodItem}>
                 <TouchableOpacity
                   onPress={() => {
                     if(this.state.mood4Color == "#888888")
                     {
                       this.setState({mood4Color: "#FE480F88"})
                     }
                     else
                     {
                       this.setState({mood4Color: "#888888"})
                     }
                   }}
                 >
                   <View style= {styles.moodIcon}>
                    <Icon1 name= "sad-tear" size= {55} color= {this.state.mood4Color}/>
                   </View>
                </TouchableOpacity>
                   <View style= {styles.moodFeeling}>
                    <Text style= {styles.textMood}> Sad </Text>
                   </View>
                 </View>
                 {/*Angry*/}
                 <View style= {styles.moodItem}>
                 <TouchableOpacity
                   onPress={() => {
                     if(this.state.mood5Color == "#888888")
                     {
                       this.setState({mood5Color: "#FE2C0F88"})
                     }
                     else
                     {
                       this.setState({mood5Color: "#888888"})
                     }
                   }}
                 >
                   <View style= {styles.moodIcon}>
                    <Icon1 name= "angry" size= {55} color= {this.state.mood5Color}/>
                   </View>
                  </TouchableOpacity>
                   <View style= {styles.moodFeeling}>
                    <Text style= {styles.textMood}> Angry </Text>
                   </View>
                 </View>
               </View>
              </View>
            </View>
          </View>
        </ScrollView>
        {/*Bottom Tabs*/}
        <Footer>
          <FooterTab>
            <Button
              onPress={() => this.props.navigation.navigate({routeName: 'HomeScreen'})}
            >
              <Icon1 name="home" size= {25} color= {"#66bb77"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooterHighlight}>Home</Text>
              </View>
            </Button>
            <Button
              onPress={() => this.props.navigation.navigate({routeName: 'Assessment'})}
            >
              <Icon1 name="clipboard" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Assessment</Text>
              </View>
            </Button>
            <Button
            style= {{paddingTop: 20}}
            onPress={() => this.props.navigation.navigate({routeName: 'BrainGames'})}
            >
              <Icon1 name="brain" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
                <Text style= {styles.textFooter}>Brain</Text>
                <Text style= {styles.textFooter}>Games</Text>
              </View>
            </Button>
            <Button
            style= {{paddingTop: 20}}
            onPress={() => this.props.navigation.navigate({routeName: 'ActiveListening'})}
            >
              <Icon1 name="assistive-listening-systems" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
                <Text style= {styles.textFooter}>Active</Text>
                <Text style= {styles.textFooter}>Listening</Text>
              </View>
            </Button>
            <Button
              onPress={() => this.props.navigation.navigate({routeName: 'Newsroom'})}
            >
              <Icon1 name="newspaper" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
                <Text style= {styles.textFooter}>Newsroom</Text>
              </View>
            </Button>
          </FooterTab>
        </Footer>
      </Container>
    );
  }
}
