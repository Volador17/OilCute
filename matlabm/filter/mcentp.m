%��ֵ���Ļ���������У��������Ʒ���������У������Ʒ����ʹ�ó���mcent��
% input 
% x:����Ⱦ���
% mx��ƽ�����ס�
% output
% xc������������Ⱦ���
function [xc]=mcentp(x,mx)
	[m,n]=size(x);
	xc=x-mx*ones(1,n);
end