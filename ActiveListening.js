import React, { Component } from 'react';
import { Platform, TouchableOpacity, Text, TextInput, StyleSheet, View, Modal, Image, ScrollView, TouchableWithoutFeedback, Dimensions } from 'react-native';
import { Container, Header, Left, Right, Body, Title, Content, Footer, FooterTab, Icon, Button} from 'native-base';
import Icon1 from 'react-native-vector-icons/FontAwesome5';
import Icon2 from 'react-native-vector-icons/MaterialCommunityIcons';
import AsyncStorage from '@react-native-community/async-storage';
import styles from '../styles/HomeScreenStyleSheet';
import stylesLandscape from '../styles/HomeScreenLandscape';

export default class ActiveListening extends Component {
  constructor(props) {
    super(props);
    this.state= {
      //screen: Dimensions.get('window'),
      modal: 'false',
      modalColor: "#66bb77",
      notificationColor: "#66bb77",
      menu: 'false',
      username: '',
    }
  }
  setUsername = async () => {
    const userData = await AsyncStorage.getItem('userData');
    const parseData = JSON.parse(userData);
    const user = parseData.userId;
    this.setState({username: user});
  }
  clearDataToStorage = (token, userId) => {
    AsyncStorage.setItem('userData', JSON.stringify({token: token, userId: userId}));
  };
  
  render() {
    return (
      <Container>
        <Modal
          animationType="fade"
          transparent={true}
          visible={this.state.modal}
          supportedOrientations={['portrait', 'landscape']}
        >
          <TouchableOpacity
            style={styles.modalContainer}
            activeOpacity={1}
            onPressOut={() => {this.setState({modal: false, notificationColor: "black"})}}
          >
              <TouchableWithoutFeedback>
                <View style={styles.modal}>
                  <View style={styles.modalArrow}>
                    <Icon2 name="triangle" size= {30} color= {"white"} style= {{paddingTop: 15}}/>
                  </View>
                  <View style={styles.modalContent}>
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
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
        <Modal
          animationType="slide"
          transparent={true}
          visible={this.state.menu}
          supportedOrientations={['portrait', 'landscape']}
        >
          <View style={styles.menuContainer}>
            <View style={styles.menu}>
              <View style={styles.menuClose}>
                <Icon2 name="close" size= {40} color= {"black"} onPress= {() => this.setState({menu: false})} style={{paddingRight: 10, paddingBottom: 5}}/>
              </View>
              <View style={styles.menuProfile}>
                <View style={styles.menuProfileImage}>
                  <Icon1 name="user" size= {100} color= {"black"}/>
                </View>
                <View style={styles.menuProfileName}>
                  <Text style={styles.textMenuProfile}> {this.state.username} </Text>
                  <TouchableOpacity
                    onPress={() => {
                      this.clearDataToStorage(null, null);
                      this.props.navigation.navigate({routeName: 'Login'});
                      this.setState({menu: false});
                    }}
                  >
                    <Text style={styles.textMenuSignOut}> Sign Out </Text>
                  </TouchableOpacity>
                </View>
              </View>
              <View style={styles.menuContent}>
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#66bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Analysis </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#66bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Settings </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#66bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Help </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#66bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> What We Do </Text>
                    </View>
                  </TouchableOpacity>
                </View>
              </View>
            </View>
          </View>
        </Modal>
        <Header>
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
        <Content>
          <View>
            <View>
            </View>
          </View>
        </Content>
        <Footer>
          <FooterTab>
            <Button
              onPress={() => this.props.navigation.navigate({routeName: 'HomeScreen'})}
            >
              <Icon1 name="home" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Home</Text>
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
              <Icon1 name="assistive-listening-systems" size= {25} color= {"#66bb77"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooterHighlight}>Active</Text>
              <Text style= {styles.textFooterHighlight}>Listening</Text>
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
